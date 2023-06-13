using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzMaps.Core.Projections
{
    public class Wgs84Projection : ProjectionBase
    {
        private readonly double DegreesToRadiansRatio = 180d / Math.PI;
        private readonly double RadiansToDegreesRatio = Math.PI / 180d;

        public Wgs84Projection(int tileSize = 256)
        {
            this.TileSize = tileSize;
        }

        public int TileSize { get; }

        public override GeoCoordinates ConvertCoordinatesToGeoCoordinates(Coordinates pixel, int zoom)
        {
            var pixelGlobeSize = (int)(this.TileSize * Math.Pow(2d, zoom));

            var xPixelsToDegreesRatio = pixelGlobeSize / 360d;
            var yPixelsToRadiansRatio = pixelGlobeSize / (2d * Math.PI);
            var halfPixelGlobeSize = Convert.ToSingle(pixelGlobeSize / 2d);

            var pixelGlobeCenter = new Coordinates((int)halfPixelGlobeSize, (int)halfPixelGlobeSize);

            var longitude = (pixel.X - pixelGlobeCenter.X) / xPixelsToDegreesRatio;

            var latitude = (2 * Math.Atan(Math.Exp((pixel.Y - pixelGlobeCenter.Y) / -yPixelsToRadiansRatio))
                - Math.PI / 2) * DegreesToRadiansRatio;

            return new GeoCoordinates { Latitude = latitude, Longitude = longitude };
        }

        public override Coordinates ConvertGeoCoordinatesToCoordinates(GeoCoordinates geoCoordinates, int zoom)
        {
            var pixelGlobeSize = (int)(this.TileSize * Math.Pow(2d, zoom));

            var xPixelsToDegreesRatio = pixelGlobeSize / 360d;
            var yPixelsToRadiansRatio = pixelGlobeSize / (2d * Math.PI);
            var halfPixelGlobeSize = Convert.ToSingle(pixelGlobeSize / 2d);

            var pixelGlobeCenter = new Coordinates((int)halfPixelGlobeSize, (int)halfPixelGlobeSize);

            var x = Math.Round(pixelGlobeCenter.X + (geoCoordinates.Longitude * xPixelsToDegreesRatio));
            var f = Math.Min(Math.Max(Math.Sin(geoCoordinates.Latitude * RadiansToDegreesRatio), -0.9999d), 0.9999d);

            var y = Math.Round(pixelGlobeCenter.Y + .5d * Math.Log((1d + f) / (1d - f)) * -yPixelsToRadiansRatio);
            return new Coordinates(Convert.ToInt32(x), Convert.ToInt32(y));
        }
    }
}
