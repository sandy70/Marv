using System;
using System.Collections.Generic;
using System.Linq;
using Smile;

namespace Marv.Common.Graph
{
    public class DistributionEvidenceString : EvidenceStringParser
    {
        public override Dictionary<string, double> Parse(Vertex vertex, string str)
        {
            var vertexValue = new Dictionary<string, double>();

            foreach (var state in vertex.States)
            {
                vertexValue[state.Key] = 0;
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
                        foreach (var state in vertex.States.Where(state => state.Range.Bounds(value)))
                        {
                            vertexValue[state.Key] += probability;
                        }
                    }
                    else
                    {
                        foreach (var state in vertex.States.Where(state => state.Key == partsOfPart[0]))
                        {
                            vertexValue[state.Key] += probability;
                        }
                    }
                }
                else
                {
                    throw new SmileException("");
                }
            }

            return vertexValue;
        }
    }
}