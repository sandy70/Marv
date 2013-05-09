using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibPipeline
{
    public interface ILocation
    {
        double Latitude { get; set; }
        double Longitude { get; set; }
    }
}
