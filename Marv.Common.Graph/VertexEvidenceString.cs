namespace Marv.Common.Graph
{
    public class VertexEvidenceString : IVertexEvidence
    {
        public VertexEvidenceString(string aString)
        {
            this.String = aString;
        }

        public string String { get; set; }

        public bool Set(Vertex vertex)
        {
            vertex.EvidenceString = this.String;
            var evidence = EvidenceStringFactory.Create(this.String).Parse(vertex.States, this.String);

            if (evidence != null)
            {
                vertex.Evidence = evidence;
                return true;
            }

            return false;
        }
    }
}