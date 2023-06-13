using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzMaps.Core
{
    public class Coordinates
    {
        public int X { get; set; }

        public int Y { get; set; }

        public Coordinates()
        {
            
        }

        public Coordinates(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
