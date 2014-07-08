using System;
using System.Collections.Generic;
using System.Linq;

namespace Marv.Common.Graph
{
    public class RangeEvidenceString : IEvidenceStringParser
    {
        public Dictionary<string, double> Parse(IEnumerable<State> states, string str)
        {
            if (str.Length <= 0) return null;

            var evidence = new Dictionary<string, double>();
            
            var parts = str
                .Trim()
                .Split(":".ToArray(), StringSplitOptions.RemoveEmptyEntries);

            if (parts.Count() != 2) return null;

            double minValue;
            double maxValue;

            if (Double.TryParse(parts[0], out minValue) && Double.TryParse(parts[1], out maxValue))
            {
                foreach (var state in states)
                {
                    evidence[state.Key] = 0;

                    if (maxValue < state.Min)
                    {
                        // do nothing
                    }
                    else if (minValue > state.Max)
                    {
                        // do nothing
                    }
                    else
                    {
                        if (state.Min <= minValue && minValue <= state.Max)
                        {
                            evidence[state.Key] = (state.Max - minValue)/(state.Max - state.Min);
                        }

                        if (state.Min <= maxValue && maxValue <= state.Max)
                        {
                            evidence[state.Key] = (maxValue - state.Min)/(state.Max - state.Min);
                        }

                        if (minValue <= state.Min && maxValue >= state.Max)
                        {
                            evidence[state.Key] = 1;
                        }
                    }
                }
            }
            else
            {
                return null;
            }

            return evidence.Normalized();
        }
    }
}