using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzMaps.Core.Projections
{
    public abstract class ProjectionBase
    {
        public abstract Coordinates ConvertGeoCoordinatesToCoordinates(GeoCoordinates geoCoordinates,  int zoom);

        public abstract GeoCoordinates ConvertCoordinatesToGeoCoordinates(Coordinates geoCoordinates, int zoom);
    }
}
