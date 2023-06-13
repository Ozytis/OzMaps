namespace OzMaps.Core.GeoJson
{
    public record MultiLineString : IGeoJson, IStylable
    {
        public List<LineString> Coordinates { get; set; }

        public virtual bool Equals(MultiLineString other)
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
