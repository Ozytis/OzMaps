using OzMaps.Core.Projections;

namespace OzMaps.Core
{
    public class Map : LayerGroup
    {
        public GeoCoordinates Center { get; set; }

        public int Zoom { get; set; } = 7;

        public int Width { get; set; }

        public int Height { get; set; }

        public ProjectionBase Projection { get; set; } = new Wgs84Projection();


        public Map(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        public List<ITileLayer> TileLayers { get; } = new();

        public void AddTileLayer(ITileLayer tileLayer)
        {
            this.TileLayers.Add(tileLayer);
        }


        public Coordinates GetAbsoluteTopLeftCoordinates()
        {
            Coordinates center = this.Projection.ConvertGeoCoordinatesToCoordinates(this.Center, this.Zoom);
            return new Coordinates(center.X - this.Width / 2, center.Y - this.Height / 2); // new zero
        }

        public Coordinates GetAbsoluteBottomRightCoordinates()
        {
            Coordinates center = this.Projection.ConvertGeoCoordinatesToCoordinates(this.Center, this.Zoom);
            return new Coordinates(center.X + this.Width / 2, center.Y + this.Height / 2); 
        }

        public Coordinates ToMapCoordinates(GeoCoordinates coordinates)
        {
            Coordinates topLeft = this.GetAbsoluteTopLeftCoordinates();

            Coordinates absCoord = this.Projection.ConvertGeoCoordinatesToCoordinates(coordinates, this.Zoom);

            return new Coordinates(absCoord.X - topLeft.X, absCoord.Y - topLeft.Y);
        }

        public GeoCoordinates ToGeoCoordinates(Coordinates coordinates)
        {
            Coordinates topLeft = this.GetAbsoluteTopLeftCoordinates();
           
            GeoCoordinates geoCoordinates = this.Projection.ConvertCoordinatesToGeoCoordinates(new Coordinates
            {
                X = coordinates.X + topLeft.X,
                Y = coordinates.Y + topLeft.Y
            }, this.Zoom);

            return geoCoordinates;
        }

        public List<(string, Legend[])> LegendSections { get; set; } = new();
        public bool ShowNorth { get; set; }

        public void AddLegendSection(string title, Legend[] legends)
        {
            this.LegendSections.Add((title, legends));
        }       
    }
}