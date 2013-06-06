using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibBn
{
    public class GraphValue : Dictionary<string, VertexValue>
    {
        public VertexValue GetVertexValue(string vertexKey)
        {
            if (this.HasVertexValue(vertexKey))
            {
                return this[vertexKey];
            }
            else
            {
                return this[vertexKey] = new VertexValue();
            }
        }

        public bool HasVertexValue(string vertexKey)
        {
            return this.ContainsKey(vertexKey);
        }
    }
}
