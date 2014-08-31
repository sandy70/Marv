using System;
using System.Collections.Generic;

namespace Marv.Common.Graph
{
    public class VertexData
    {
        public double[] Beliefs { get; set; }
        public string String { get; set; }
        public double[] Evidence { get; set; }

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