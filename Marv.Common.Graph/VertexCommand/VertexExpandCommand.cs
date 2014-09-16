namespace Marv
{
    public class VertexExpandCommand : Command<Vertex>
    {
        public override void Excecute(Vertex vertex)
        {
            vertex.IsExpanded = !vertex.IsExpanded;
        }
    }
}