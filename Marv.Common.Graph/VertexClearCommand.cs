namespace Marv
{
    public class VertexClearCommand : Command<Vertex>
    {
        public override void Excecute(Vertex vertex)
        {
            base.Excecute(vertex);
            vertex.Evidence = null;
            vertex.EvidenceString = null;
        }
    }
}