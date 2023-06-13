using OzMaps.Core;
using OzMaps.Core.GeoJson;
using OzMaps.Core.TileLayers;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzMaps.Web.Models
{
    public class PrintModel
    {
        public decimal Width { get; set; }

        public decimal Height { get; set; }

        public decimal Dpi { get; set; }

        [Required]
        public GeoCoordinates Center { get; set; }

        public List<TileLayer> TileLayers { get; set; }

        public List<GeoJsonModel> GeoJsons { get; set; }

        public List<MapMarker> Markers { get; set; }

        public int Zoom { get; set; }

        public List<LegendSectionModel> Legends { get; set; }

        public PrintType Type { get; set; } = PrintType.Pdf;

        public bool ShowNorth { get; set; } = true;
    }
}
