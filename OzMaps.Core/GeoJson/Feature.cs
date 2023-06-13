namespace OzMaps.Core.GeoJson
{
    public record Feature : IGeoJson, IStylable
    {
        public IGeoJson Geometry { get; set; }

        public Dictionary<string, object> Properties { get; set; }

        public GeoJsonStyle Style { get; set; }
    }
}
