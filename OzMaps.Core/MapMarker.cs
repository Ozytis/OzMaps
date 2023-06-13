using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzMaps.Core
{
    public class MapMarker
    {
        public string Icon { get; set; } = "https://www.svgrepo.com/show/302636/map-marker.svg";

        public GeoCoordinates Coordinates { get; set; }

        public int Width { get; set; } = 20;

        public int Height { get; set; } = 20;
    }
}
