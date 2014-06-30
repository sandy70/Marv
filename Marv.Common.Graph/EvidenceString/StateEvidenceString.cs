using System;
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

            if (states.Count(state => state.Key == str) == 1)
            {
                evidence[str] = 1;
            }
            else
            {
                double value;

                if (Double.TryParse(str, out value))
                {
                    var isWithinBounds = false;

                    foreach (var state in states.Where(s => s.Contains(value)))
                    {
                        evidence[state.Key] = 1;
                        isWithinBounds = true;
                    }

                    if (!isWithinBounds)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }

            return evidence.Normalized();
        }
    }
}