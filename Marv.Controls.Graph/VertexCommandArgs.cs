using Marv.Common;
using Marv.Common.Graph;

namespace Marv.Controls.Graph
{
    public class VertexCommandArgs
    {
        public Command<Vertex> Command { get; set; }
        public Vertex Vertex { get; set; }
    }
}