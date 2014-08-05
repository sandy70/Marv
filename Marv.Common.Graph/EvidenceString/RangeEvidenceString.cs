using System;
using System.Collections.Generic;
using System.Linq;

namespace Marv.Common.Graph
{
    public class RangeEvidenceString : IEvidenceStringParser
    {
        public Dictionary<string, double> Parse(IEnumerable<State> states, string str)
        {
            var values = EvidenceStringFactory.ParseArray(str, ":");

            if (values == null || values.Length != 2) return null;

            // Sort the values
            Array.Sort(values);
            
            var dist = new UniformDistribution(values[0], values[1]);

            return states.Parse(dist);
        }
    }
}