using System.Collections.Generic;

namespace Marv.Common.Graph
{
    public interface IEvidenceStringParser
    {
        Dictionary<string, double> Parse(Vertex vertex, string str);
    }
}