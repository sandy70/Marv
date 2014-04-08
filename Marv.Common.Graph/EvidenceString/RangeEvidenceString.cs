using System;
using System.Collections.Generic;
using System.Linq;

namespace Marv.Common.Graph
{
    public class RangeEvidenceString : IEvidenceStringParser
    {
        public Dictionary<string, double> Parse(Vertex vertex, string str)
        {
            if (str.Length <= 0) return null;

            var evidence = vertex.CreateEvidence();

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
                    evidence[state.Key] = 0;

                    if (maxValue < state.Range.Min)
                    {
                        // do nothing
                    }
                    else if (minValue > state.Range.Max)
                    {
                        // do nothing
                    }
                    else
                    {
                        if (minValue >= state.Range.Min && minValue <= state.Range.Max)
                        {
                            evidence[state.Key] = (state.Range.Max - minValue) / (state.Range.Max - state.Range.Min);
                        }

                        if (maxValue >= state.Range.Min && maxValue <= state.Range.Max)
                        {
                            evidence[state.Key] = (maxValue - state.Range.Min) / (state.Range.Max - state.Range.Min);
                        }

                        if (minValue <= state.Range.Min && maxValue >= state.Range.Max)
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
