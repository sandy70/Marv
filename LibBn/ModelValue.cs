using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibBn
{
    public class ModelValue : Dictionary<string, GraphValue>
    {
        public GraphValue GetGraphValue(string graphName)
        {
            if (this.HasGraphValue(graphName))
            {
                return this[graphName];
            }
            else
            {
                return this[graphName] = new GraphValue();
            }
        }

        public void SetGraphValue(string graphName, GraphValue graphValue)
        {
            this[graphName] = graphValue;
        }

        public bool HasGraphValue(string graphName)
        {
            return this.ContainsKey(graphName);
        }
    }
}
