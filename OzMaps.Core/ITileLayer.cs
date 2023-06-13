namespace OzMaps.Core
{
    public interface ITileLayer
    {
        public string Url { get; }

        public string Attribution { get; }

        public int TileSize { get;  }

        public decimal Opacity { get; set; }
    }
}
