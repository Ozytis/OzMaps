using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzMaps.Core.TileLayers
{
    public class OpenStreetMapLayer : ITileLayer
    {
        public OpenStreetMapLayer()
        {
            this.Url = "https://tile.openstreetmap.org/{z}/{x}/{y}.png";
            this.Attribution = "https://tile.openstreetmap.org/{z}/{x}/{y}.png";
            this.TileSize = 256;
        }

        public string Url { get; private set; }

        public string Attribution { get; private set; }

        public int TileSize { get; private set; }

        public decimal Opacity { get; set; } = 1;
    }
}
