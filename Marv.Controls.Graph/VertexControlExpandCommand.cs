namespace Marv.Controls.Graph
{
    public class VertexControlExpandCommand : Command<VertexControl>
    {
        public override void Excecute(VertexControl vertexControl)
        {
            vertexControl.Vertex.IsExpanded = !vertexControl.Vertex.IsExpanded;
            vertexControl.IsStatesVisible = !vertexControl.IsStatesVisible;
        }
    }
}