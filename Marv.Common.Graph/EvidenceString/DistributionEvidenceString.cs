using System;
using System.Collections.Generic;
using System.Linq;
using Smile;

namespace Marv.Common.Graph
{
    public class DistributionEvidenceString : EvidenceString
    {
        public DistributionEvidenceString(string aString)
            : base(aString)
        {
        }

        public override Dictionary<string, double> Parse(Vertex vertex)
        {
            var evidence = vertex.ToEvidence();

            foreach (var stateKey in evidence.Keys)
            {
                evidence[stateKey] = 0;
            }

            var parts = this._string
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
                        foreach (var state in vertex.States.Where(state => state.Range.Bounds(value)))
                        {
                            evidence[state.Key] += probability;
                        }
                    }
                    else
                    {
                        foreach (var state in vertex.States.Where(state => state.Key == partsOfPart[0]))
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