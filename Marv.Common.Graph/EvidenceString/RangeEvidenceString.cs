using System;
using System.Collections.Generic;
using System.Linq;

namespace Marv.Common.Graph
{
    public class RangeEvidenceString : IEvidenceStringParser
    {
        public Dictionary<string, double> Parse(IEnumerable<State> states, string str)
        {
            var parts = str
                .Trim()
                .Split(":".ToArray(), StringSplitOptions.RemoveEmptyEntries);

            var values = new double[parts.Length];

            for (var i = 0; i < parts.Length; i++)
            {
                // Return null if any of these cannot be parsed
                if (!double.TryParse(parts[i], out values[i])) values = null;
            }

            if (values == null || values.Length != 2) return null;

            // If values are in reverse order then swap
            if (values[0] > values[1])
            {
                var temp = values[0];
                values[0] = values[1];
                values[1] = temp;
            }

            var dist = new UniformDistribution(values[0], values[1]);

            return states.Parse(dist);
        }
    }
}