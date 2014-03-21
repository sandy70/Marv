using System;
using System.Collections.Generic;
using System.Linq;

namespace Marv.Common.Graph
{
    public class RangeEvidenceString : EvidenceString
    {
        public RangeEvidenceString(string aString)
            : base(aString)
        { }

        public override Dictionary<string, double> Parse(Vertex vertex)
        {
            var evidence = new Dictionary<string, double>();

            var parts = this._string
                            .Trim()
                            .Split(":".ToArray(), StringSplitOptions.RemoveEmptyEntries);

            double minValue;
            double maxValue;

            if (Double.TryParse(parts[0], out minValue) && Double.TryParse(parts[1], out maxValue))
            {
                foreach (var state in vertex.States)
                {
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
                throw new Smile.SmileException("");
            }

            return evidence;
        }
    }
}
