using System;
using System.Collections.Generic;

namespace Marv.Common
{
    public class VertexStandardDeviationComputer : IVertexValueComputer
    {
        public double Compute(Vertex vertex, Dictionary<string, double> vertexValue)
        {
            var stdev = 0.0;

            var meanComputer = new VertexMeanComputer();

            var mu = meanComputer.Compute(vertex, vertexValue);

            foreach (var state in vertex.States)
            {
                stdev += vertexValue[state.Key] *
                         (
                         1.0 / 3 * (Math.Pow(state.Range.Max, 3) - Math.Pow(state.Range.Min, 3)) +
                         Math.Pow(mu, 2) * (state.Range.Max - state.Range.Min) -
                         mu * (Math.Pow(state.Range.Max, 2) - Math.Pow(state.Range.Min, 2))
                         );
            }

            return Math.Sqrt(stdev / vertex.States.Count);
        }
    }
}