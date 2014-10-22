namespace Marv.Controls.Graph
{
    public class VertexControlSubGraphCommand : Command<VertexControl>
    {
        public override void Excecute(VertexControl vertexControl)
        {
            var graphControl = vertexControl.GetParent<GraphControl>();

            if (graphControl != null)
            {
                graphControl.UpdateDisplayGraph(vertexControl.Vertex.HeaderOfGroup, vertexControl.Vertex.Key);
            }
        }
    }
}