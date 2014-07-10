using System;
using System.Collections.Generic;
using System.Linq;

namespace Marv.Common.Graph
{
    public class DistributionEvidenceString : IEvidenceStringParser
    {
        public Dictionary<string, double> Parse(IEnumerable<State> states, string str)
        {
            var evidence = new Dictionary<string, double>();

            var parts = str
                .Trim()
                .Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);

            var stateList = states as IList<State> ?? states.ToList();

            if (parts.Count() < stateList.Count() || parts.Count() > stateList.Count())
            {
                return null;
            }

            for (var i = 0; i < parts.Count(); i++)
            {
                double value;

                if (Double.TryParse(parts[i], out value))
                {
                    var stateKey = stateList.ElementAt(i).Key;
                    evidence[stateKey] = value;
                }
                else
                {
                    return null;
                }
            }

            return evidence;
        }
    }
}