using Marv.Common;
using Marv.Controls.Graph;

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