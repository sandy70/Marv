namespace Marv.Controls.Graph
{
    public class VertexControlExpandCommand : Command<VertexControl>
    {
        public override void Excecute(VertexControl vertexControl)
        {
            vertexControl.IsExpanded = !vertexControl.IsExpanded;
            vertexControl.IsStatesVisible = !vertexControl.IsStatesVisible;
            vertexControl.IsMostProbableStateVisible = !vertexControl.IsMostProbableStateVisible;
        }
    }
}