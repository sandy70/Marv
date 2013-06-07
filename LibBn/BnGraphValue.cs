using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibBn
{
    public class BnGraphValue
    {
        private Dictionary<string, BnVertexValue> vertexValuesByKey = new Dictionary<string, BnVertexValue>();

        public BnVertexValue GetVertexValue(string vertexKey)
        {
            return this.vertexValuesByKey[vertexKey];
        }

        public bool HasVertexValue(string vertexKey)
        {
            return this.vertexValuesByKey.ContainsKey(vertexKey);
        }

        public void SetVertexValue(string vertexKey, BnVertexValue vertexValue)
        {
            this.vertexValuesByKey[vertexKey] = vertexValue;
        }
    }
}
