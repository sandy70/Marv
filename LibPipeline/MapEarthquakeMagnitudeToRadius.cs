using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibPipeline
{
    class MapEarthquakeMagnitudeToRadius : IMapDoubleToDouble
    {
        public double Map(double a)
        {
            return a * 10;
        }


        public double MapBack(double b)
        {
            return b / 10;
        }
    }
}
