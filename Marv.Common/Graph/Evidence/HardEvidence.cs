namespace Marv.Common
{
    public class HardEvidence : IEvidence
    {
        public int StateIndex { get; set; }

        public string String { get; set; }

        public string GetValue(Graph graph, string vertexKey)
        {
            return graph.Vertices[vertexKey].States[this.StateIndex].ValueString;
        }

        public void Set(Graph graph, string vertexKey)
        {
            graph.SetEvidence(vertexKey, this.StateIndex);
        }
    }
}