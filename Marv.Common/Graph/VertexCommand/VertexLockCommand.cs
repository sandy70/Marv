namespace Marv.Common
{
    public class VertexLockCommand : VertexCommand
    {
        public override void RaiseExecuted(Vertex vertex)
        {
            vertex.IsLocked = !vertex.IsLocked;

            if (vertex.IsLocked)
            {
                this.ImageSource = "/Marv.Common;component/Resources/Icons/Lock.png";
            }
            else
            {
                this.ImageSource = "/Marv.Common;component/Resources/Icons/Unlock.png";
            }

            base.RaiseExecuted(vertex);
        }
    }
}