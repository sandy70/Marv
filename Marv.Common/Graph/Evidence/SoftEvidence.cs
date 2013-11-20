using System.Linq;
using MoreLinq;

namespace Marv.Common
{
    public class SoftEvidence : IEvidence
    {
        public double[] Evidence { get; set; }

        public string String { get; set; }

        public void Set(Graph graph, string vertexKey)
        {
            graph.SetEvidence(vertexKey, this.Evidence);
        }

        public string GetValue(Graph graph, string vertexKey)
        {
            var maxStateIndex = this.Evidence.MaxIndex();
            return graph.Vertices[vertexKey].States[maxStateIndex].ValueString + this.String.Enquote('{', '}');
        }
    }
}