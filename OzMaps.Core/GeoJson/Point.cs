using System.Globalization;

namespace OzMaps.Core.GeoJson
{
    public record Point : GeoCoordinates, IGeoJson
    {
        public Point()
        {

        }

        public Point(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }     

        public override string ToString()
        {
            return $"{this.Latitude.ToString(CultureInfo.InvariantCulture)},{this.Longitude.ToString(CultureInfo.InvariantCulture)}";
        }

        public static Point operator -(Point point1, Point point2)
        {
            return new Point(point1.Latitude - point2.Latitude, point1.Longitude - point2.Longitude);
        }

        public Point Round()
        {
            return new Point
            {
                Latitude = Math.Round(this.Latitude),
                Longitude = Math.Round(this.Longitude)
            };
        }

        public Point ScaleBy(Point point)
        {
            return new Point(this.Longitude * point.Longitude, this.Latitude * point.Latitude);
        }

        public Point ScaleBy(double x, double y)
        {
            return new Point(this.Longitude * x, this.Latitude * y);
        }

        public Point UnscaleBy(Point point)
        {
            return new Point(this.Longitude / point.Longitude, this.Latitude / point.Latitude);
        }

        public virtual bool Equals(Point other)
        {
            return other != null && this.Latitude == other.Latitude && this.Longitude == other.Longitude;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
