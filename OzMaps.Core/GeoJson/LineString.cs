namespace OzMaps.Core.GeoJson
{
    public record LineString : IGeoJson, IStylable
    {
        public List<Point> Coordinates { get; set; }

        public bool IsClosed()
        {
            if (this.Coordinates == null)
            {
                return false;
            }

            var firstCoordinate = this.Coordinates[0];
            var lastCoordinate = this.Coordinates[this.Coordinates.Count - 1];

            return firstCoordinate.Longitude.Equals(lastCoordinate.Longitude)
                   && firstCoordinate.Latitude.Equals(lastCoordinate.Latitude);
        }

        public bool IsLinearRing()
        {
            return this.Coordinates != null && this.Coordinates.Count >= 4 && this.IsClosed();
        }

        public virtual bool Equals(LineString other)
        {
            return other?.Coordinates != null && this.Coordinates != null && this.Coordinates.SequenceEqual(other.Coordinates);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public GeoJsonStyle Style { get; set; }

    }
}
