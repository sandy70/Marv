using System.Collections.Generic;

namespace Marv.Common.Graph
{
    public interface IVertexValueComputer
    {
        double Compute(Vertex vertex, Dictionary<string, double> vertexValue);
    }
}