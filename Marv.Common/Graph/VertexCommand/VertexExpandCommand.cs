﻿namespace Marv.Common
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