using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzMaps.Core
{
    public record GeoCoordinates
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double DistanceWith(GeoCoordinates geoCoordinates)
        {
            double radius = 6378.16;

            double dlon = Radians(geoCoordinates.Longitude - this.Longitude);
            double dlat = Radians(geoCoordinates.Latitude - this.Latitude);

            double a = (Math.Sin(dlat / 2) * Math.Sin(dlat / 2)) + Math.Cos(Radians(this.Latitude)) * Math.Cos(Radians(geoCoordinates.Latitude)) * (Math.Sin(dlon / 2) * Math.Sin(dlon / 2));
            double angle = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return angle * radius;
        }

        private static double Radians(double x)
        {
            return x * Math.PI / 180;
        }
    }
}
