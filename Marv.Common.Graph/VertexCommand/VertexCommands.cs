namespace Marv
{
    public static class VertexCommands
    {
        public static readonly VertexExpandCommand Expand = new VertexExpandCommand
        {
            ImageSource = "/Marv.Common;component/Resources/Icons/Expand.png"
        };

        public static readonly VertexLockCommand Lock = new VertexLockCommand
        {
            ImageSource = "/Marv.Common;component/Resources/Icons/Lock.png"
        };

        public static readonly Command<Vertex> SubGraph = new Command<Vertex>
        {
            ImageSource = "/Marv.Common;component/Resources/Icons/SubGraph.png"
        };

        public static readonly Command<Vertex> Clear = new VertexClearCommand
        {
            ImageSource = "/Marv.Common;component/Resources/Icons/Clear.png"
        };
    }
}