using System.Collections.Generic;
using System.Linq;

namespace Marv.Common.Graph
{
    public class DistributionEvidenceString : IEvidenceStringParser
    {
        public Dictionary<string, double> Parse(IEnumerable<State> states, string str)
        {
            var stateList = states as IList<State> ?? states.ToList();

            var values = EvidenceStringFactory.ParseArray(str);

            if (values == null || values.Length != stateList.Count) return null;

            var evidence = new Dictionary<string, double>();

            for (var i = 0; i < stateList.Count; i++)
            {
                evidence[stateList[i].Key] = values[i];
            }

            return evidence.Normalized();
        }
    }
}