using Marv.Common;

namespace Marv.Controls
{
    public static class VertexControlCommands
    {
        public static readonly VertexControlExpandCommand Expand = new VertexControlExpandCommand
        {
            ImageSource = "/Marv.Common;component/Resources/Icons/Expand.png"
        };

        public static readonly Command<VertexControl> SubGraph = new Command<VertexControl>
        {
            ImageSource = "/Marv.Common;component/Resources/Icons/SubGraph.png"
        };
    }
}