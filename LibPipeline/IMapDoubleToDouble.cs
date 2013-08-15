using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibPipeline
{
    interface IMapDoubleToDouble
    {
        double Map(double a);
        double MapBack(double b);
    }
}
