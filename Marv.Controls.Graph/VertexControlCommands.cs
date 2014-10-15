using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marv.Controls.Graph
{
    public static class VertexControlCommands
    {
        public static readonly VertexControlExpandCommand Expand = new VertexControlExpandCommand
        {
            ImageSource = "/Marv.Common;component/Resources/Icons/Expand.png"
        };

        public static VertexControlSubGraphCommand SubGraph = new VertexControlSubGraphCommand
        {
            ImageSource = "/Marv.Common;component/Resources/Icons/SubGraph.png"
        };
    }
}
