using System.Collections.Generic;
using System.Linq;

namespace Marv.Common.Graph
{
    public class StateEvidenceString : IEvidenceStringParser
    {
        public Dictionary<string, double> Parse(IEnumerable<State> states, string str)
        {
            if (str.Length <= 0) return null;

            var evidence = new Dictionary<string, double>();

            var stateList = states as IList<State> ?? states.ToList();

            if (stateList.Any(state => state.Key == str))
            {
                evidence[str] = 1;
            }
            else
            {
                double value;

                if (double.TryParse(str, out value))
                {
                    var dist = new DeltaDistribution(value);
                    return stateList.Parse(dist);
                }

                return null;
            }

            return evidence.Normalized();
        }
    }
}