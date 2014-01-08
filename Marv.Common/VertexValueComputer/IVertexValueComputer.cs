using System.Collections.Generic;

namespace Marv.Common
{
    public interface IVertexValueComputer
    {
        double Compute(Vertex vertex, Dictionary<string, double> vertexValue);
    }
}