using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibNetwork
{
    public class VertexExpandCommand : VertexCommand
    {
        public override void Execute(VertexViewModel vertexViewModel)
        {
            base.Execute(vertexViewModel);
            vertexViewModel.IsExpanded = !vertexViewModel.IsExpanded;
        }
    }
}
