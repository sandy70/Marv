using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marv.Common.Graph
{
    public static class VertexCommands
    {
        public static VertexExpandCommandNew VertexExpandCommand = new VertexExpandCommandNew
        {
            ImageSource = "/Marv.Common;component/Resources/Icons/Expand.png"
        };

        public static VertexLockCommandNew VertexLockCommand = new VertexLockCommandNew
        {
            ImageSource = "/Marv.Common;component/Resources/Icons/Lock.png"
        };
    }
}
