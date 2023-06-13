using OzMaps.Core.GeoJson;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzMaps.Core
{
    public class LayerGroup
    {
        public List<MapMarker> Markers { get; } = new();

        public void AddMarker(MapMarker marker)
        {
            this.Markers.Add(marker);
        }

        public List<LayerGroup> LayerGroups { get; } = new();

        public void AddLayerGroup(LayerGroup layerGroup)
        {
            this.LayerGroups.Add(layerGroup);
        }

        public List<IGeoJson> GeoJsons { get; } = new();

        public void AddGeoJson(IGeoJson geoJson)
        {
            this.GeoJsons.Add(geoJson);
        }
    }
}
