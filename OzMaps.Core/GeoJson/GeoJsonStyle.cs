using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzMaps.Core.GeoJson
{
    public class GeoJsonStyle
    {
        public string StrokeColor { get; set; } = "#000";

        public string FillColor { get; set; } = "#FFFFFFFF";

        public string FillColor2 { get; set; } = "#FFFFFFFF";

        public int StrokeWidth { get; set; } = 1;

        public string FillPattern { get; set; }

        public decimal Opacity { get; set; } = 1;
    }
}
