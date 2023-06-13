using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzMaps.Core.TileLayers
{
    public class TileLayer : ITileLayer
    {
        public string Url { get; set; }

        public string Attribution { get; set; }

        public int TileSize { get; set; } = 256;

        public decimal Opacity { get; set; }
    }
}
