using System.Collections.Generic;

namespace Marv.Common.Graph
{
    public class Evidence
    {
        public string String { get; set; }
        public Dictionary<string, double> Value { get; set; }

        public void Normalize()
        {
            this.Value = this.Value.Normalized();
        }
    }
}