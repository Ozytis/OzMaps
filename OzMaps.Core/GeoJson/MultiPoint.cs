namespace OzMaps.Core.GeoJson
{
    public class MultiPoint : IGeoJson
    {
        public List<Point> Coordinates { get; set; }

        public virtual bool Equals(MultiPoint other)
        {
            return other?.Coordinates != null && this.Coordinates != null && this.Coordinates.SequenceEqual(other.Coordinates);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
