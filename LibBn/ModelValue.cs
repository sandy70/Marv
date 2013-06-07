using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibBn
{
    public class ModelValue : Dictionary<string, BnGraphValue>
    {
        public BnGraphValue GetGraphValue(string graphName)
        {
            if (this.HasGraphValue(graphName))
            {
                return this[graphName];
            }
            else
            {
                return this[graphName] = new BnGraphValue();
            }
        }

        public void SetGraphValue(string graphName, BnGraphValue graphValue)
        {
            this[graphName] = graphValue;
        }

        public bool HasGraphValue(string graphName)
        {
            return this.ContainsKey(graphName);
        }
    }
}
