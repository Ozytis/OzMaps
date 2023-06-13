using OzMaps.Core.GeoJson;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OzMaps.Core
{
    public static class SvgMapRenderer
    {
        public static async Task RenderAsSvgAsync(this Map map, string outputFile)
        {
            SvgRenderer svgRenderer = new SvgRenderer(map);
            await svgRenderer.RenderAsync(outputFile);
        }

        public static async Task<byte[]> RenderAsSvgAsync(this Map map)
        {
            SvgRenderer svgRenderer = new SvgRenderer(map);
            return await svgRenderer.RenderAsync();
        }

        public static string RenderAsSvgString(this Map map)
        {
            SvgRenderer svgRenderer = new SvgRenderer(map);
            return svgRenderer.RenderAsString();
        }
    }

    public class SvgRenderer
    {
        public static XNamespace SvgNamespace = "http://www.w3.org/2000/svg";
        public static XNamespace HtmlNamespace = "http://www.w3.org/1999/xhtml";

        public Map Map { get; }

        public SvgRenderer(Map map)
        {
            this.Map = map;
        }

        public string RenderAsString()
        {
            XDocument xDocument = this.BuildSvg();
            return xDocument.ToString();
        }

        public async Task<byte[]> RenderAsync()
        {

            XDocument xDocument = this.BuildSvg();
            MemoryStream stream = new MemoryStream();
            await xDocument.SaveAsync(stream, SaveOptions.None, new CancellationToken());
            return stream.ToArray();
        }

        public async Task RenderAsync(string outputFile)
        {
            XDocument xDocument = this.BuildSvg();
            using StreamWriter writer = new StreamWriter(outputFile);
            await xDocument.SaveAsync(writer, SaveOptions.None, new CancellationToken());
        }

        public XDocument BuildSvg()
        {

            XDocument svg = new XDocument(new XDeclaration("1.0", "utf-8", "no"),
               new XElement(SvgNamespace + "svg", new XAttribute("width", this.Map.Width), new XAttribute("height", this.Map.Height)));

            XElement svgTileLayersGroups = new XElement(SvgNamespace + "g", new XAttribute("id", "tilelayers"));

            foreach (var tileLayer in this.Map.TileLayers)
            {
                var svgLayer = RenderTileLayer(tileLayer, svg.Root);

                svgTileLayersGroups.Add(svgLayer);
            }

            svg.Root.Add(svgTileLayersGroups);

            svg.Root.Add(this.RenderLayerGroup(this.Map, svg.Root));

            svg.Root.Add(this.RenderLegend(this.Map, svg.Root));

            if (this.Map.ShowNorth)
            {
                svg.Root.Add(this.RenderNorthIndication(svg.Root));
            }

            svg.Root.Add(this.RenderScale(svg.Root));

            svg.Root.Add(new XElement(HtmlNamespace + "script",
                new XAttribute("type", "text/javascript"),
                "window.print();"
            ));

            return svg;
        }

        private XElement RenderScale(XElement root)
        {
            string color = "#000000";

            XElement group = new XElement(SvgNamespace + "g",
                new XAttribute("transform", $"translate(10, {Math.Round(this.Map.Height * 0.95).ToString("0")})"));


            var topLeft = this.Map.ToGeoCoordinates(new Coordinates(0, 0));
            var topRight = this.Map.ToGeoCoordinates(new Coordinates(0, this.Map.Width));

            var distance = topLeft.DistanceWith(topRight);

            double scale = 1;

            if (this.Map.Zoom > 8)
            {
                scale = scale * 0.1;
            }

            


            int width = (int)Math.Round(this.Map.Width * scale / distance);

            XElement path = new XElement(SvgNamespace + "path",
                new XAttribute("stroke", color),
                new XAttribute("fill", "transparent"),
                new XAttribute("d", $"M5 0, L 5 5, L{width + 5} 5, L {width + 5} 0"));

            group.Add(path);

            XElement legend = new XElement(SvgNamespace + "text",
                new XAttribute("y", "5"),
                new XAttribute("x", $"{width + 15}"),
                new XAttribute("font-size", "8"), $"{(scale >= 1 ? "1km" : Math.Round(1000 * scale) + "m")}");
            group.Add(legend);

            return group;
        }

        private XElement RenderNorthIndication(XElement root)
        {
            string color = "#FF6600";
            string color2 = "#FFFFFF";
            int size = (int)Math.Round(this.Map.Width * 0.05);

            XElement group = new XElement(SvgNamespace + "g", new XAttribute("transform", "translate(10,16)"));
            group.Add(new XElement(SvgNamespace + "text", new XAttribute("color", color), new XAttribute("x", size / 2 - 4), "N"));


            XElement star = new XElement(SvgNamespace + "g", new XAttribute("transform", "translate(0,4)"));



            star.Add(new XElement(SvgNamespace + "path", new XAttribute("d", $"M{size / 8} {size / 8},L{size / 2 - size / 16} {size / 4}, L {size / 8 * 3} {size / 8 * 3}, L{size / 8} {size / 8}"), new XAttribute("fill", color2), new XAttribute("stroke", color)));
            star.Add(new XElement(SvgNamespace + "path", new XAttribute("d", $"M{size / 8} {size / 8},L{size / 4} {size / 2 - size / 16}, L {size / 8 * 3} {size / 8 * 3}, L{size / 8} {size / 8}"), new XAttribute("fill", color), new XAttribute("stroke", color)));
            star.Add(new XElement(SvgNamespace + "path", new XAttribute("d", $"M{size / 8 * 7} 4, L{size / 8 * 6} {size / 8 * 3}, L {size / 8 * 5} {size / 8 * 3}, L{size / 8 * 7} {size / 8}"), new XAttribute("fill", color2), new XAttribute("stroke", color)));
            star.Add(new XElement(SvgNamespace + "path", new XAttribute("d", $"M{size / 8 * 7} {size / 8}, L{size / 2 + size / 16} {size / 4}, L {size / 8 * 5} {size / 8 * 3}, L{size / 8 * 7} {size / 8}"), new XAttribute("fill", color), new XAttribute("stroke", color)));
            star.Add(new XElement(SvgNamespace + "path", new XAttribute("d", $"M{size / 8 * 7} {size / 8 * 7}, L{size / 8 * 5} {size / 8 * 6}, L {size / 8 * 5} {size / 8 * 5}, L{size / 8 * 7} {size / 8 * 7}"), new XAttribute("fill", color2), new XAttribute("stroke", color)));
            star.Add(new XElement(SvgNamespace + "path", new XAttribute("d", $"M{size / 8 * 7} {size / 8 * 7}, L{size / 8 * 6} {size / 2 + size / 16}, L {size / 8 * 5} {size / 8 * 5}, L{size / 8 * 7} {size / 8 * 7}"), new XAttribute("fill", color), new XAttribute("stroke", color)));
            star.Add(new XElement(SvgNamespace + "path", new XAttribute("d", $"M{size / 8} {size / 8 * 7}, L{size / 4} {size / 2 + size / 16}, L {size / 8 * 3} {size / 8 * 5}, L{size / 8} {size / 8 * 7}"), new XAttribute("fill", color2), new XAttribute("stroke", color)));
            star.Add(new XElement(SvgNamespace + "path", new XAttribute("d", $"M{size / 8} {size / 8 * 7}, L{size / 2 - size / 16} {size / 8 * 6}, L {size / 8 * 3} {size / 8 * 5}, L{size / 8} {size / 8 * 7}"), new XAttribute("fill", color), new XAttribute("stroke", color)));

            star.Add(new XElement(SvgNamespace + "path", new XAttribute("d", $"M{size / 2} 0, L{size / 8 * 5} {size / 8 * 3}, L{size / 2} {size / 2}, L {size / 2} 0"), new XAttribute("fill", color2), new XAttribute("stroke", color)));
            star.Add(new XElement(SvgNamespace + "path", new XAttribute("d", $"M{size / 2} 0, L{size / 2} {size / 2}, L{size / 8 * 3} {size / 8 * 3}, L{size / 2} 0"), new XAttribute("fill", color), new XAttribute("stroke", color)));
            star.Add(new XElement(SvgNamespace + "path", new XAttribute("d", $"M{size} {size / 2}, L{size / 8 * 5} {size / 8 * 5}, L{size / 2} {size / 2}, L{size} {size / 2}"), new XAttribute("fill", color2), new XAttribute("stroke", color)));
            star.Add(new XElement(SvgNamespace + "path", new XAttribute("d", $"M{size} {size / 2}, L{size / 8 * 5} {size / 8 * 3}, L{size / 2} {size / 2}, L{size} {size / 2}"), new XAttribute("fill", color), new XAttribute("stroke", color)));
            star.Add(new XElement(SvgNamespace + "path", new XAttribute("d", $"M{size / 2} {size}, L{size / 8 * 3} {size / 8 * 3}, L{size / 2} {size / 2}, L{size / 2} {size}"), new XAttribute("fill", color2), new XAttribute("stroke", color)));
            star.Add(new XElement(SvgNamespace + "path", new XAttribute("d", $"M{size / 2} {size}, L{size / 8 * 5} {size / 8 * 5}, L{size / 2} {size / 2}, L{size / 2} {size}"), new XAttribute("fill", color), new XAttribute("stroke", color)));
            star.Add(new XElement(SvgNamespace + "path", new XAttribute("d", $"M0 {size / 2}, L{size / 8 * 3} {size / 8 * 5}, L{size / 2} {size / 2}, L0 {size / 2}"), new XAttribute("fill", color), new XAttribute("stroke", color)));
            star.Add(new XElement(SvgNamespace + "path", new XAttribute("d", $"M0 {size / 2}, L{size / 8 * 3} {size / 8 * 3}, L{size / 2} {size / 2}, L0 {size / 2}"), new XAttribute("fill", color2), new XAttribute("stroke", color)));


            group.Add(star);

            return group;
        }

        private XElement RenderLegend(Map map, XElement root)
        {
            if (map.LegendSections == null || map.LegendSections.Count == 0)
            {
                return root;
            }

            XElement group = new XElement(SvgNamespace + "foreignObject",
                new XAttribute("height", this.Map.Height),
                new XAttribute("width", this.Map.Width));

            XElement panel = new XElement(HtmlNamespace + "div");

            string style = "background-color:#fff;color:#000;position:absolute;right:0;bottom:0;padding:15px;opacity:.85";
            panel.Add(new XAttribute("style", style));
            group.Add(panel);

            panel.Add(new XElement(HtmlNamespace + "div",
                new XAttribute("style", "font-weight:bold;font-family:sans-serif;"),
                "Légende"));

            int y = 25;

            foreach (var section in map.LegendSections)
            {

                var sectionHeader = new XElement(HtmlNamespace + "div",
                    new XAttribute("style", "display:flex; text-decoration:underline;margin-top:6px;font-family:sans-serif;font-size:small"),
                    section.Item1);

                panel.Add(sectionHeader);

                foreach (var mapLegend in section.Item2)
                {
                    var row = new XElement(HtmlNamespace + "div", new XAttribute("style", "display:flex; margin-top:3px;align-items:center"));

                    var picto = new XElement(HtmlNamespace + "div", new XAttribute("style", "display:flex;border:solid 1px #000;width:20px;height:20px;"));

                    picto.Add(this.CreateLegendPictogram(mapLegend));

                    row.Add(picto);

                    var legend = new XElement(HtmlNamespace + "div", new XAttribute("style", "margin-left:6px;font-family:sans-serif;font-size:small"), mapLegend.Title);
                    row.Add(legend);

                    panel.Add(row);
                }



            }

            return group;
        }

        public XElement CreateLegendPictogram(Legend legend)
        {
            XElement svg = new XElement(SvgNamespace + "svg",
                new XAttribute("viewBox", "0 0 50 50"),
                new XAttribute("preserveAspectRatio", "none"));

            var defs = new XElement(SvgNamespace + "defs");

            XElement svgPattern = XElement.Parse(legend.Pictogram.FillPattern.Replace("{1}", legend.Pictogram.FillColor).Replace("{2}", legend.Pictogram.FillColor2));

            if (svgPattern.Attribute("xmlns") != null)
            {
                svgPattern.Attribute("xmlns").Remove();
            }

            var svgPattern2 = ReplaceNameSpace(svgPattern, SvgNamespace);

            svgPattern2.SetAttributeValue("id", "a" + Guid.NewGuid());

            defs.Add(svgPattern2);

            svg.Add(defs);

            var rect = new XElement(SvgNamespace + "rect",
                 new XAttribute("x", "0"),
                 new XAttribute("y", "0"),
                 new XAttribute("height", "500"),
                 new XAttribute("width", "200"),
                 new XAttribute("fill", $"url(#{svgPattern2.Attribute("id").Value})"));

            svg.Add(rect);

            return svg;
        }

        public XElement RenderLayerGroup(LayerGroup layerGroup, XElement svgRoot)
        {
            XElement svgGroup = new XElement(SvgNamespace + "g");

            foreach (var mapMarker in layerGroup.Markers)
            {
                var svgmarker = this.RenderMarker(mapMarker, svgRoot);
                svgGroup.Add(svgmarker);
            }

            foreach (var geoJson in layerGroup.GeoJsons)
            {
                var svgShape = this.RenderGeoJson(geoJson, svgRoot);
                svgGroup.Add(svgShape);
            }

            return svgGroup;
        }

        public XElement RenderGeoJson(IGeoJson geoJson, XElement svgRoot)
        {
            switch (geoJson)
            {
                case GeoJson.LineString line: return RenderLine(line, svgRoot);
                case GeoJson.MultiLineString multiLine: return RenderMultiLine(multiLine, svgRoot);
                case GeoJson.Polygon polygon: return RenderPolygon(polygon, svgRoot);
                case GeoJson.MultiPolygon multiPolygon: return RenderMultiPolygon(multiPolygon, svgRoot);
                case Feature feature:

                    if (feature.Geometry is IStylable stylable)
                    {
                        stylable.Style ??= feature.Style;
                    }

                    return RenderGeoJson(feature.Geometry, svgRoot);
                default:
                    return null;
            }
        }

        public XElement RenderMultiPolygon(GeoJson.MultiPolygon multiPolygon, XElement svgRoot)
        {
            XElement svgPolygon = new XElement(SvgNamespace + "g");

            foreach (var polygon in multiPolygon.Coordinates)
            {
                polygon.Style ??= multiPolygon.Style;
                svgPolygon.Add(RenderPolygon(polygon, svgRoot));
            }

            return svgPolygon;
        }

        public XElement RenderPolygon(GeoJson.Polygon polygon, XElement svgRoot)
        {
            if (polygon.Coordinates == null || !polygon.Coordinates.Any())
            {
                return null;
            }

            var style = polygon.Style ?? new GeoJsonStyle();

            XElement svgPolygon = new XElement(SvgNamespace + "path",
                new XAttribute("stroke", style.StrokeColor),
                new XAttribute("opacity", style.Opacity.ToString("0.##", CultureInfo.InvariantCulture)),
                new XAttribute("stroke-width", style.StrokeWidth.ToString("0.##", CultureInfo.InvariantCulture)));

            var start = this.Map.ToMapCoordinates(polygon.ExteriorRing.Coordinates.First());

            StringBuilder points = new($"M {start.X} {start.Y}");

            foreach (var coordinate in polygon.ExteriorRing.Coordinates)
            {
                var mapPoint = this.Map.ToMapCoordinates(coordinate);
                points.Append($" L {mapPoint.X},{mapPoint.Y}");
            }

            // on ferme le polygone
            points.Append($"z");

            if (polygon.Holes != null)
            {
                foreach (var hole in polygon.Holes)
                {
                    var startHole = this.Map.ToMapCoordinates(hole.Coordinates.First());
                    points.Append($" M {startHole.X} {startHole.Y}");

                    foreach (var coordinate in hole.Coordinates)
                    {
                        var mapPoint = this.Map.ToMapCoordinates(coordinate);
                        points.Append($" L {mapPoint.X},{mapPoint.Y}");
                    }

                    points.Append("z");
                }
            }

            svgPolygon.Add(new XAttribute("d", points.ToString()));

            if (!string.IsNullOrEmpty(style?.FillPattern))
            {
                XElement svgPattern = XElement.Parse(polygon.Style?.FillPattern.Replace("{1}", style.FillColor).Replace("{2}", style.FillColor2));

                if (svgPattern.Attribute("xmlns") != null)
                {
                    svgPattern.Attribute("xmlns").Remove();
                }

                var svgPattern2 = ReplaceNameSpace(svgPattern, SvgNamespace);

                svgPattern2.SetAttributeValue("id", "a" + Guid.NewGuid());

                svgRoot.Add(svgPattern2);

                svgPolygon.Add(new XAttribute("fill", $"url(#{svgPattern2.Attribute("id").Value})"));
            }
            else
            {
                svgPolygon.Add(new XAttribute("fill", style.FillColor));
            }

            return svgPolygon;

        }

        public XElement ReplaceNameSpace(XElement element, XNamespace newNamespace)
        {
            XElement result = new XElement(newNamespace + element.Name.LocalName);
            result.Add(element.Attributes());

            foreach (var child in element.Elements())
            {
                result.Add(ReplaceNameSpace(child, newNamespace));
            }

            return result;
        }

        public XElement RenderMultiLine(GeoJson.MultiLineString multiLine, XElement svgRoot)
        {
            if (multiLine.Coordinates == null || multiLine.Coordinates.Count == 0)
            {
                return null;
            }

            XElement svgMultiLine = new XElement(SvgNamespace + "g");

            foreach (var line in multiLine.Coordinates)
            {
                line.Style ??= multiLine.Style;

                svgMultiLine.Add(RenderLine(line, svgRoot));
            }

            return svgMultiLine;
        }

        public XElement RenderLine(GeoJson.LineString line, XElement svgRoot)
        {
            if (line.Coordinates == null || !line.Coordinates.Any())
            {
                return null;
            }

            var start = this.Map.ToMapCoordinates(line.Coordinates.First());

            StringBuilder path = new($"");

            foreach (var point in line.Coordinates)
            {
                var mapPoint = this.Map.ToMapCoordinates(point);
                path.Append($"{mapPoint.X},{mapPoint.Y} ");
            }

            var style = line.Style ?? new();

            return new XElement(SvgNamespace + "polyline",
                new XAttribute("points", path.ToString()),
                new XAttribute("stroke", style.StrokeColor),
                new XAttribute("fill", style.FillColor),
                new XAttribute("stroke-width", style.StrokeWidth.ToString("0.##", CultureInfo.InvariantCulture)));
        }

        public XElement RenderMarker(MapMarker marker, XElement svgRoot)
        {
            Coordinates coordinates = this.Map.ToMapCoordinates(marker.Coordinates);

            XElement svgmarker = new XElement(SvgNamespace + "g");
            svgmarker.Add(new XElement(SvgNamespace + "image",
                new XAttribute("href", marker.Icon),
                new XAttribute("x", coordinates.X - marker.Width / 2),
                new XAttribute("y", coordinates.Y - marker.Height),
                new XAttribute("width", marker.Width),
                new XAttribute("height", marker.Height)
            ));

            return svgmarker;
        }

        public XElement RenderTileLayer(ITileLayer tileLayer, XElement svgRoot)
        {

            // on récupère le coin en haut à gauche
            var topLeft = this.Map.Projection.ConvertCoordinatesToGeoCoordinates(this.Map.GetAbsoluteTopLeftCoordinates(), this.Map.Zoom);

            // on récupère la tuile liée au coin en haut à gauche
            var topLeftTileXNumber = Longitude2TileX(topLeft.Longitude, this.Map.Zoom);
            var topLeftTileYNumber = Latitude2TileY(topLeft.Latitude, this.Map.Zoom);

            var bottomRight = this.Map.Projection.ConvertCoordinatesToGeoCoordinates(this.Map.GetAbsoluteBottomRightCoordinates(), this.Map.Zoom);

            var bottomRightTileXNumber = Longitude2TileX(bottomRight.Longitude, this.Map.Zoom);
            var bottomRightTileYNumber = Latitude2TileY(bottomRight.Latitude, this.Map.Zoom);

            // on récupère l
            // es vraies coordonnées de cette tuile
            var trueTopLeftTileOrigin = new GeoCoordinates
            {
                Longitude = TileX2Longitude(topLeftTileXNumber, this.Map.Zoom),
                Latitude = TileY2Latitude(topLeftTileYNumber, this.Map.Zoom)
            };

            // on récupère le décalage en pixel
            var offset = this.Map.ToMapCoordinates(trueTopLeftTileOrigin);

            XElement svglayer = new XElement(SvgNamespace + "g",
                new XAttribute("style", $"opacity:{tileLayer.Opacity.ToString("0.##", CultureInfo.InvariantCulture)}"));


            int nbTileH = (int)Math.Ceiling(this.Map.Width / (double)tileLayer.TileSize);
            int nbTileY = (int)Math.Ceiling(this.Map.Height / (double)tileLayer.TileSize);

            for (int x = topLeftTileXNumber; x <= bottomRightTileXNumber; x++)
            {
                for (int y = topLeftTileYNumber; y <= bottomRightTileYNumber; y++)
                {
                    var geoPoint = new GeoCoordinates
                    {
                        Longitude = TileX2Longitude(x, this.Map.Zoom),
                        Latitude = TileY2Latitude(y, this.Map.Zoom)
                    };

                    var coordinates = this.Map.ToMapCoordinates(geoPoint);

                    svglayer.Add(new XElement(SvgNamespace + "image",
                        new XAttribute("href", GetTileUrl(geoPoint, tileLayer, 0, 0)),
                        new XAttribute("x", coordinates.X),
                        new XAttribute("y", coordinates.Y),
                        new XAttribute("width", tileLayer.TileSize),
                        new XAttribute("height", tileLayer.TileSize)
                    ));
                }
            }

            return svglayer;
        }

        public string GetTileUrl(GeoCoordinates coordinates, ITileLayer tileLayer, int xOffset = 0, int yoffset = 0)
        {
            return tileLayer.Url.Replace("{z}", this.Map.Zoom.ToString("0"))
                .Replace("{s}.", string.Empty)
                .Replace("{x}", (Longitude2TileX(coordinates.Longitude, Map.Zoom) + xOffset).ToString("0"))
                .Replace("{y}", (Latitude2TileY(coordinates.Latitude, Map.Zoom) + yoffset).ToString("0"));
        }

        private static int Longitude2TileX(double longitude, int zoom)
        {
            return (int)(Math.Floor((longitude + 180.0) / 360.0 * (1 << zoom)));
        }

        private static int Latitude2TileY(double latitude, int zoom)
        {
            return (int)Math.Floor((1 - Math.Log(Math.Tan(ToRadians(latitude)) + 1 / Math.Cos(ToRadians(latitude))) / Math.PI) / 2 * (1 << zoom));
        }

        private static double TileX2Longitude(int x, int zoom)
        {
            return x / (double)(1 << zoom) * 360.0 - 180;
        }

        private static double TileY2Latitude(int y, int zoom)
        {
            double n = Math.PI - 2.0 * Math.PI * y / (double)(1 << zoom);
            return 180.0 / Math.PI * Math.Atan(0.5 * (Math.Exp(n) - Math.Exp(-n)));
        }

        public static double ToRadians(double angle)
        {
            return angle * Math.PI / 180;
        }
    }
}
