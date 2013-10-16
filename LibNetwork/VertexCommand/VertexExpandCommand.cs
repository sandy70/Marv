using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibNetwork
{
    public class VertexExpandCommand : VertexCommand
    {
        public override void RaiseExecuted(Vertex vertexViewModel)
        {
            vertexViewModel.IsExpanded = !vertexViewModel.IsExpanded;

            base.RaiseExecuted(vertexViewModel);
        }
    }
}
