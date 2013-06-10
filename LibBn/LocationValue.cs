using System.Collections.Generic;

namespace LibBn
{
    public class LocationValue : Dictionary<int, ModelValue>
    {
        public ModelValue GetModelValue(int year)
        {
            if (this.ContainsKey(year))
            {
                return this[year];
            }
            else
            {
                return this[year] = new ModelValue();
            }
        }
    }
}