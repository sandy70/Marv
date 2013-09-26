using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibPipeline
{
    class MapDoubleToDouble : IMapDoubleToDouble
    {
        public double Map(double a)
        {
            return a;
        }


        public double MapBack(double b)
        {
            return b;
        }
    }
}
