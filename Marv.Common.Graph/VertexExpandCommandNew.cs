namespace Marv.Common.Graph
{
    public class VertexExpandCommandNew : Command<Vertex>
    {
        public override void Excecute(Vertex vertex)
        {
            vertex.IsExpanded = !vertex.IsExpanded;
        }
    }
}