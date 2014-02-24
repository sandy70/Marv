namespace Marv.Common.Graph
{
    public class HardEvidence : IEvidence
    {
        public int StateIndex { get; set; }

        public string SynergiString { get; set; }

        public void Set(Graph graph, string vertexKey)
        {
            graph.SetEvidence(vertexKey, this.StateIndex);
        }
    }
}