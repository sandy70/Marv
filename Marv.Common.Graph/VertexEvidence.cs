using System;
using System.Collections.Generic;
using System.Linq;

namespace Marv.Common.Graph
{
    public class VertexEvidence
    {
        public string String { get; private set; }
        public double[] Values { get; private set; }

        public VertexEvidence() {}

        public VertexEvidence(IEnumerable<double> values, string str)
        {
            this.Values = values.ToArray();
            this.String = str;
        }

        public static List<double> ParseValues(string str)
        {
            var values = new List<double>();

            var parts = str.Trim().Split("(),: ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                double value;
                if (double.TryParse(part, out value)) values.Add(value);
            }

            return values;
        }

        public override string ToString()
        {
            return this.String;
        }
    }
}