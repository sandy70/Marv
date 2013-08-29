namespace LibNetwork
{
    public class VertexEvidence
    {
        public double[] Evidence { get; set; }

        public EvidenceType EvidenceType { get; set; }

        public string Key { get; set; }

        public int StateIndex { get; set; }

        public virtual void SetOnVertex(BnVertex vertex) {}
    }
}