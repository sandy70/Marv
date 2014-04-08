using System;
using System.Collections.Generic;
using System.Linq;
using Smile;

namespace Marv.Common.Graph
{
    public class StateEvidenceString : EvidenceStringParser
    {
        public override Dictionary<string, double> Parse(Vertex vertex, string str)
        {
            if (str.Length <= 0) return null;

            var evidence = vertex.CreateEvidence();

            if (vertex.States.Count(state => state.Key == str) == 1)
            {
                evidence[str] = 1;
            }
            else
            {
                double value;

                if (Double.TryParse(str, out value) && vertex.Type == VertexType.Interval)
                {
                    var isWithinBounds = false;

                    foreach (var state in vertex.States.Where(state => state.Range.Bounds(value)))
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