using System;
using System.Collections.Generic;
using System.Linq;

namespace Marv.Common.Graph
{
    public class StateEvidenceString : IEvidenceStringParser
    {
        public Evidence Parse(Vertex vertex, string str)
        {
            if (str.Length <= 0) return null;

            var evidence = new Evidence();
            evidence.String = str;
            evidence.Value = new Dictionary<string, double>();

            if (vertex.States.Count(state => state.Key == str) == 1)
            {
                evidence.Value[str] = 1;
            }
            else
            {
                double value;

                if (Double.TryParse(str, out value) && vertex.Type == VertexType.Interval)
                {
                    var isWithinBounds = false;

                    foreach (var state in vertex.States.Where(state => state.Contains(value)))
                    {
                        evidence.Value[state.Key] = 1;
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

            evidence.Value = evidence.Value.Normalized();
            return evidence;
        }
    }
}