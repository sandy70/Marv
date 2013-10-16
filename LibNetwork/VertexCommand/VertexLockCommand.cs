using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibNetwork
{
    public class VertexLockCommand : VertexCommand
    {
        public override void Execute(VertexViewModel vertexViewModel)
        {
            vertexViewModel.IsLocked = !vertexViewModel.IsLocked;

            base.Execute(vertexViewModel);

            if (vertexViewModel.IsLocked)
            {
                this.ImageSource = "/Marv.Common;component/Resources/Icons/Lock.png";
            }
            else
            {
                this.ImageSource = "/Marv.Common;component/Resources/Icons/Unlock.png";
            }
        }
    }
}
