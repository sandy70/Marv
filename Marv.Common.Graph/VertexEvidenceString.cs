namespace Marv.Common.Graph
{
    public class VertexEvidenceString : IVertexEvidence
    {
        public string String { get; set; }

        public VertexEvidenceString(string aString)
        {
            this.String = aString;
        }

        public bool Set(Vertex vertex)
        {
            vertex.EvidenceString = this.String;
            var evidenceStringParser = EvidenceStringFactory.Create(this.String);
            var evidence = evidenceStringParser.Parse(vertex.States, this.String);

            if (evidence != null)
            {
                vertex.Evidence = evidence;
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return this.String;
        }
    }
}