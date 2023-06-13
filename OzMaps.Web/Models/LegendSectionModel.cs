using OzMaps.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzMaps.Web.Models
{
    public class LegendSectionModel
    {
        public string Name { get; set; }

        public Legend[] Legends { get; set; }
    }
}
