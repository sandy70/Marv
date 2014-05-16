using System;
using System.Collections.Generic;
using System.Linq;

namespace Marv.Common.Graph
{
    public class RangeEvidenceString : IEvidenceStringParser
    {
        public Evidence Parse(Vertex vertex, string str)
        {
            if (str.Length <= 0) return null;

            var evidence = new Evidence();
            evidence.String = str;

            var parts = str
                .Trim()
                .Split(":".ToArray(), StringSplitOptions.RemoveEmptyEntries);

            if (parts.Count() != 2) return null;

            double minValue;
            double maxValue;

            if (Double.TryParse(parts[0], out minValue) && Double.TryParse(parts[1], out maxValue))
            {
                foreach (var state in vertex.States)
                {
                    evidence.Value[state.Key] = 0;

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
                        if (minValue >= state.Min && minValue <= state.Max)
                        {
                            evidence.Value[state.Key] = (state.Max - minValue)/(state.Max - state.Min);
                        }

                        if (maxValue >= state.Min && maxValue <= state.Max)
                        {
                            evidence.Value[state.Key] = (maxValue - state.Min)/(state.Max - state.Min);
                        }

                        if (minValue <= state.Min && maxValue >= state.Max)
                        {
                            evidence.Value[state.Key] = 1;
                        }
                    }
                }
            }
            else
            {
                return null;
            }

            evidence.Value = evidence.Value.Normalized();

            return evidence;
        }
    }
}