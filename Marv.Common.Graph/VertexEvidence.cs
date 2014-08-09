using System;
using System.Collections.Generic;

namespace Marv.Common.Graph
{
    public class VertexEvidence
    {
        public double[] Evidence { get; set; }
        public string String { get; set; }

        public VertexEvidence() {}

        public VertexEvidence(double[] evidence, string str)
        {
            this.Evidence = evidence;
            this.String = str;
        }

        public override string ToString()
        {
            return this.String;
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
    }
}