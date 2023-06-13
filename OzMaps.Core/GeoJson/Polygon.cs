namespace OzMaps.Core.GeoJson
{
    public record Polygon : IGeoJson, IStylable
    {
        public List<LineString> Coordinates { get; set; }

        public GeoJsonStyle Style { get; set; }

        public LineString ExteriorRing
        {
            get
            {
                return this.Coordinates?.FirstOrDefault();
            }
            internal set
            {
                this.Coordinates ??= new List<LineString>();

                if (value != null)
                {

                    if (this.Coordinates.Any())
                    {
                        this.Coordinates[0] = value;
                    }
                    else
                    {
                        this.Coordinates.Add(value);
                    }
                }
            }
        }

        public List<LineString> Holes
        {
            get
            {
                if (this.Coordinates == null || this.Coordinates.Count < 2)
                {
                    return null;
                }

                return this.Coordinates?.Skip(1).ToList();
            }

            internal set
            {
                this.Coordinates ??= new List<LineString>();

                if (value != null)
                {
                    if (!this.Coordinates.Any())
                    {
                        this.Coordinates.Add(new());
                    }

                    var result = new List<LineString>
                    {
                        this.Coordinates[0]
                    };

                    result.AddRange(value);

                    this.Coordinates = result;
                }
            }
        }

        public virtual bool Equals(Polygon other)
        {
            return other?.Coordinates != null && this.Coordinates != null && this.Coordinates.SequenceEqual(other.Coordinates);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
