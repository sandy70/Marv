namespace LibNetwork
{
    public class HardEvidence : IEvidence
    {
        public int StateIndex { get; set; }

        public void Set(Graph graph, string vertexKey)
        {
            graph.SetEvidence(vertexKey, this.StateIndex);
        }
    }
}