using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marv.Common
{
    public class VertexLockCommand : VertexCommand
    {
        public override void RaiseExecuted(Vertex vertexViewModel)
        {
            vertexViewModel.IsLocked = !vertexViewModel.IsLocked;

            if (vertexViewModel.IsLocked)
            {
                this.ImageSource = "/Marv.Common;component/Resources/Icons/Lock.png";
            }
            else
            {
                this.ImageSource = "/Marv.Common;component/Resources/Icons/Unlock.png";
            }

            base.RaiseExecuted(vertexViewModel);
        }
    }
}
