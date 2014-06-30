using System.Collections.Generic;

namespace Marv.Common.Graph
{
    public class VertexEntropyDifferenceComputer : IVertexValueComputer
    {
        public double Compute(Vertex vertex, Dictionary<string, double> vertexValue)
        {
            var vertexEntropyComputer = new VertexEntropyComputer();

            var oldEntropy = vertex.GetStatistics("Entropy", vertexEntropyComputer);
            var newEntropy = vertexEntropyComputer.Compute(vertex, vertexValue);

            return newEntropy - oldEntropy;
        }
    }
}