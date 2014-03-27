namespace Marv.Common.Graph
{
    public class VertexLockCommand : Command<Vertex>
    {
        public override void Excecute(Vertex vertex)
        {
            vertex.IsLocked = !vertex.IsLocked;

            this.ImageSource = vertex.IsLocked ? "/Marv.Common;component/Resources/Icons/Lock.png"
                : "/Marv.Common;component/Resources/Icons/Unlock.png";
        }
    }
}