using System;
using System.Collections.Generic;

namespace Marv.Common.Graph
{
    public class VertexEntropyComputer : IVertexValueComputer
    {
        public double Compute(Vertex vertex, Dictionary<string, double> vertexValue)
        {
            var entropy = 0.0;

            foreach (var stateKey in vertexValue.Keys)
            {
                var stateValue = vertexValue[stateKey];

                // log(0) is NaN
                if (stateValue > 0)
                {
                    entropy += stateValue * Math.Log(stateValue);
                }
            }

            return -entropy / Math.Log(vertexValue.Keys.Count);
        }
    }
}