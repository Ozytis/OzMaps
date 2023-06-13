using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzMaps.Core
{
    public record Bounds
    {
        public GeoCoordinates NorthEast  { get; set; }

        public GeoCoordinates SouthWest  { get; set; }
    }
}
