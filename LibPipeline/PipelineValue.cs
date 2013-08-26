using LibNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibPipeline
{
    public class PipelineValue : Dictionary<Location, LocationValue>
    {
        public bool HasLocationValue(Location location)
        {
            return this.ContainsKey(location);
        }

        public LocationValue GetLocationValue(Location location)
        {
            return this[location];
        }
    }
}
