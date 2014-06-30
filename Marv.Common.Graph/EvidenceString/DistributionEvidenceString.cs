using System;
using System.Collections.Generic;
using System.Linq;
using Smile;

namespace Marv.Common.Graph
{
    public class DistributionEvidenceString : IEvidenceStringParser
    {
        public Dictionary<string, double> Parse(IEnumerable<State> states , string str)
        {
            var evidence = new Dictionary<string, double>();

            foreach (var state in states)
            {
                evidence[state.Key] = 0;
            }

            var parts = str
                .Trim()
                .Split(";".ToArray(), StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                var partsOfPart = part.Trim()
                    .Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);

                double probability;

                if (Double.TryParse(partsOfPart[1], out probability))
                {
                    double value;

                    if (Double.TryParse(partsOfPart[0], out value))
                    {
                        foreach (var state in states.Where(state => state.Contains(value)))
                        {
                            evidence[state.Key] += probability;
                        }
                    }
                    else
                    {
                        foreach (var state in states.Where(state => state.Key == partsOfPart[0]))
                        {
                            evidence[state.Key] += probability;
                        }
                    }
                }
                else
                {
                    throw new SmileException("");
                }
            }

            return evidence;
        }
    }
}