namespace OzMaps.Core.GeoJson
{
    public record FeatureCollection : IGeoJson
    {
        public List<Feature> Features { get; set; }

        public virtual bool Equals(FeatureCollection other)
        {
            return other?.Features != null && this.Features != null && this.Features.SequenceEqual(other.Features);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
