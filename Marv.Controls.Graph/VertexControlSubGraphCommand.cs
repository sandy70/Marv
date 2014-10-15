namespace Marv.Controls.Graph
{
    public class VertexControlSubGraphCommand : Command<VertexControl>
    {
        public override void Excecute(VertexControl vertexControl)
        {
            var graphControl = vertexControl.FindParent<GraphControl>();

            if (graphControl != null)
            {
                graphControl.UpdateDisplayGraph(vertexControl.Vertex.HeaderOfGroup);
            }
        }
    }
}