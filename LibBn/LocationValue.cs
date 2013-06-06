using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibBn
{
    public class LocationValue : Dictionary<int, ModelValue>
    {
        public ModelValue GetModelValue(int year)
        {
            if(this.HasModelValue(year))
            {
                return this[year];
            }
            else
            {
                return this[year] = new ModelValue();
            }
        }

        public bool HasModelValue(int year)
        {
            return this.ContainsKey(year);
        }
    }
}
