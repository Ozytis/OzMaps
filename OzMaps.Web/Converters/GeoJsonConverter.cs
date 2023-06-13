using OzMaps.Core.GeoJson;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace OzMaps.Web.Converters
{
    internal class GeoJsonConverter : JsonConverter<IGeoJson>
    {
        public override IGeoJson Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                reader.Read();
                return null;
            }

            reader.Read();
            reader.SkipComments();

            Type geojsonType = null;
            JsonElement coordinateData = default;
            JsonElement properties = default;

            IGeoJson[] geoJsons = null;
            IGeoJson geometry = null;
            GeoJsonStyle geojsonStyle = null;
            Dictionary<string, JsonElement> additionals = new();

            while (reader.TokenType == JsonTokenType.PropertyName)
            {
                // Get the property name and decide what to do
                string propertyName = reader.GetString();
                reader.Read();

                switch (propertyName)
                {
                    case "type":
                        geojsonType = GetType(reader.GetString());
                        reader.Read();
                        break;
                    case "geometries":
                        geoJsons = JsonSerializer.Deserialize<IGeoJson[]>(ref reader, options);
                        reader.Read();
                        break;
                    case "geometry":
                        geometry = JsonSerializer.Deserialize<IGeoJson>(ref reader, options);
                        reader.Read();
                        break;
                    case "coordinates":
                        coordinateData = JsonSerializer.Deserialize<JsonElement>(ref reader, options);
                        reader.Read();
                        break;
                    case "properties":
                        properties = JsonSerializer.Deserialize<JsonElement>(ref reader, options);
                        reader.Read();
                        break;
                    case "style":
                        geojsonStyle = JsonSerializer.Deserialize<GeoJsonStyle>(ref reader, options);
                        reader.Read();
                        break;
                    default:
                        additionals.Add(propertyName, JsonSerializer.Deserialize<JsonElement>(ref reader, options));
                        reader.Read();
                        break;
                }

                // Skip comments
                reader.SkipComments();
            }

            if (geojsonType == null)
            {
                return null;
            }

            IGeoJson result = Activator.CreateInstance(geojsonType) as IGeoJson;


            switch (result)
            {
                case LineString lineString:
                    lineString.Coordinates = coordinateData.Deserialize<List<Point>>();
                    break;
                case MultiPolygon multiPolygon:

                    var mpCoordinates = coordinateData.Deserialize<List<List<List<List<double>>>>>();

                    multiPolygon.Coordinates = mpCoordinates.Select(c => new Polygon
                    {
                        Coordinates = c.Select(p => new LineString
                        {
                            Coordinates = p.Select(pt => new Point
                            {
                                Latitude = pt[1],
                                Longitude = pt[0],
                            }).ToList(),
                        }).ToList()
                    }).ToList();

                    break;
                case Polygon polygon:
                    var polygonCoordinates = coordinateData.Deserialize<List<List<List<double>>>>();

                    polygon.Coordinates = polygonCoordinates.Select(p => new LineString
                    {
                        Coordinates = p.Select(pt => new Point
                        {
                            Latitude = pt[1],
                            Longitude = pt[0],
                        }).ToList(),
                    }).ToList();
                    break;
                case MultiLineString multiLineString:
                    var mlCoordinates = coordinateData.Deserialize<List<List<List<double>>>>();

                    multiLineString.Coordinates = mlCoordinates.Select(p => new LineString
                    {
                        Coordinates = p.Select(pt => new Point
                        {
                            Latitude = pt[1],
                            Longitude = pt[0],
                        }).ToList(),
                    }).ToList();
                    break;
                case Feature feature:
                    feature.Geometry = geometry;
                    break;
                case Point point:
                    if (additionals.ContainsKey(nameof(Point.Latitude)))
                    {
                        point.Latitude = additionals["Latitude"].GetDouble();
                    }

                    if (additionals.ContainsKey(nameof(Point.Latitude)))
                    {
                        point.Longitude = additionals["Longitude"].GetDouble();
                    }

                    break;
            }

            if(result is IStylable stylable)
            {
                stylable.Style = geojsonStyle;
            }

            //reader.Read();


            return result;
        }

        private Type GetType(string geometryType)
        {
            switch (geometryType)
            {
                case nameof(Point):
                    return typeof(Point);
                case nameof(LineString):
                    return typeof(LineString);
                case nameof(MultiLineString):
                    return typeof(MultiLineString);
                case nameof(Polygon):
                    return typeof(Polygon);
                case nameof(MultiPolygon):
                    return typeof(MultiPolygon);
                case nameof(Feature):
                    return typeof(Feature);
                default:
                    throw new NotSupportedException();
            }
        }

        public override void Write(Utf8JsonWriter writer, IGeoJson value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}