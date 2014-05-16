using System;
using System.Collections.Generic;
using System.Linq;
using Smile;

namespace Marv.Common.Graph
{
    public class DistributionEvidenceString : IEvidenceStringParser
    {
        public Evidence Parse(Vertex vertex, string str)
        {
            var evidence = new Evidence();
            evidence.String = str;

            foreach (var state in vertex.States)
            {
                evidence.Value[state.Key] = 0;
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
                        foreach (var state in vertex.States.Where(state => state.Contains(value)))
                        {
                            evidence.Value[state.Key] += probability;
                        }
                    }
                    else
                    {
                        foreach (var state in vertex.States.Where(state => state.Key == partsOfPart[0]))
                        {
                            evidence.Value[state.Key] += probability;
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