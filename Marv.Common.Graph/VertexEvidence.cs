namespace Marv.Common.Graph
{
    public class VertexEvidence
    {
        public double[] Evidence { get; set; }
        public string String { get; set; }

        public VertexEvidence() {}

        public VertexEvidence(double[] evidence, string str)
        {
            this.Evidence = evidence;
            this.String = str;
        }

        public override string ToString()
        {
            return this.String;
        }
    }
}