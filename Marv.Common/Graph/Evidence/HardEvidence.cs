namespace Marv.Common
{
    public class HardEvidence : IEvidence
    {
        public string EvidenceString { get; set; }

        public int StateIndex { get; set; }

        public void Set(Graph graph, string vertexKey)
        {
            graph.SetEvidence(vertexKey, this.StateIndex);
        }
    }
}