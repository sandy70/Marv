using LibBn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibPipeline
{
    public class PipelineValue : Dictionary<ILocation, LocationValue>
    {
        public bool HasLocationValue(ILocation location)
        {
            return this.ContainsKey(location);
        }

        public LocationValue GetLocationValue(ILocation location)
        {
            return this[location];
        }
    }
}
