namespace Marv.Graph
{
    public class VertexClearCommand : Command<Vertex>
    {
        public override void Excecute(Vertex vertex)
        {
            base.Excecute(vertex);
            vertex.States.ClearEvidence();
            vertex.EvidenceString = null;
        }
    }
}