using LibBn;
using System.Windows;

namespace LibPipeline
{
    public static partial class Extensions
    {
        public static void CalculateMostProbableStates(this BnGraph graph)
        {
            foreach (var vertex in graph.Vertices)
            {
                BnVertexViewModel vertexViewModel = vertex as BnVertexViewModel;

                if ((vertexViewModel != null))
                {
                    vertexViewModel.MostProbableState = vertex.GetMostProbableState();
                }
            }
        }
    }
}