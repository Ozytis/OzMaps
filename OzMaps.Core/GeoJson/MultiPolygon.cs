namespace OzMaps.Core.GeoJson
{
    public record MultiPolygon : IGeoJson, IStylable
    {
        public List<Polygon> Coordinates { get; set; }

        public GeoJsonStyle Style { get; set; } 

        public virtual bool Equals(MultiPolygon other)
        {
            return other?.Coordinates != null && this.Coordinates != null && this.Coordinates.SequenceEqual(other.Coordinates);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
