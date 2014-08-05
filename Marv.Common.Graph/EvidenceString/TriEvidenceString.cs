using System;
using System.Collections.Generic;

namespace Marv.Common.Graph
{
    internal class TriEvidenceString : IEvidenceStringParser
    {
        public Dictionary<string, double> Parse(IEnumerable<State> states, string str)
        {
            var values = EvidenceStringFactory.ParseParams(str);

            if (values == null || values.Length != 3) return null;

            // Sort values into correct order. Makes the function robust
            Array.Sort(values);

            var dist = new TriangularDistribution(values[0], values[1], values[2]);

            return states.Parse(dist);
        }
    }
}