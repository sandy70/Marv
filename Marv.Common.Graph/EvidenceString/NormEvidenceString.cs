using System.Collections.Generic;

namespace Marv.Common.Graph
{
    public class NormEvidenceString : IEvidenceStringParser
    {
        public Dictionary<string, double> Parse(IEnumerable<State> states, string str)
        {
            var values = EvidenceStringFactory.ParseParams(str);

            if (values == null || values.Length != 2) return null;

            var dist = new NormalDistribution(values[0], values[1]);

            return states.Parse(dist);
        }
    }
}