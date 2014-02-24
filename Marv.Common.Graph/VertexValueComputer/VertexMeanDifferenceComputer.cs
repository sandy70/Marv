using System.Collections.Generic;

namespace Marv.Common.Graph
{
    public class VertexMeanDifferenceComputer : IVertexValueComputer
    {
        public double Compute(Vertex vertex, Dictionary<string, double> vertexValue)
        {
            var vertexMeanComputer = new VertexMeanComputer();

            var oldMean = vertex.GetStatistics("Mean", vertexMeanComputer);
            var newMean = vertexMeanComputer.Compute(vertex, vertexValue);

            return newMean - oldMean;
        }
    }
}