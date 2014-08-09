namespace Marv.Common.Graph
{
    public class VertexClearCommand : Command<Vertex>
    {
        public override void Excecute(Vertex vertex)
        {
            base.Excecute(vertex);
            vertex.States.SetEvidence(0);
            vertex.EvidenceString = null;
        }
    }
}