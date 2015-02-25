using Marv.Common;

namespace Marv.Controls
{
    public class VertexControlExpandCommand : Command<VertexControl>
    {
        public override void Excecute(VertexControl vertexControl)
        {
            vertexControl.Vertex.IsExpanded = !vertexControl.Vertex.IsExpanded;
        }
    }
}