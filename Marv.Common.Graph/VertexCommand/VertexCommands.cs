using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marv.Common.Graph
{
    public static class VertexCommands
    {
        public static VertexExpandCommand Expand = new VertexExpandCommand
        {
            ImageSource = "/Marv.Common;component/Resources/Icons/Expand.png"
        };

        public static VertexLockCommand Lock = new VertexLockCommand
        {
            ImageSource = "/Marv.Common;component/Resources/Icons/Lock.png"
        };

        public static Command<Vertex> SubGraph = new Command<Vertex>
        {
            ImageSource = "/Marv.Common;component/Resources/Icons/SubGraph.png"
        };
    }
}
