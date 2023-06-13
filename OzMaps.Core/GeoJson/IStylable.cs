using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzMaps.Core.GeoJson
{
    public interface IStylable
    {
        public GeoJsonStyle Style { get; set; }
    }
}
