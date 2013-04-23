using QuickGraph.Algorithms.RankedShortestPath;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LibBn
{
    public class PartGraphGenerator
    {
        public BnGraph Generate(BnGraph srcGraph, string group)
        {
            // Extract the header vertices
            BnGraph partGraph = new BnGraph();

            foreach (var vertex in srcGraph.Vertices)
            {
                if (vertex.Groups.Contains(group))
                {
                    partGraph.AddVertex(vertex);

                    Point positionByGroup;

                    if (vertex.PositionsByGroup.TryGetValue(group, out positionByGroup))
                    {
                        vertex.DisplayPosition = positionByGroup;
                    }
                    else
                    {
                        vertex.PositionsByGroup.Add(group, vertex.Position);
                        vertex.DisplayPosition = vertex.Position;
                    }
                }
            }

            // Process for each pair
            foreach (var srcVertex in partGraph.Vertices)
            {
                foreach (var dstVertex in partGraph.Vertices)
                {
                    var algorithm = new HoffmanPavleyRankedShortestPathAlgorithm<BnVertex, BnEdge>(srcGraph, (edge) => { return 1; });
                    algorithm.Compute(srcVertex, dstVertex);

                    foreach (var path in algorithm.ComputedShortestPaths)
                    {
                        BnVertex src = path.First().Source;

                        foreach (var edge in path)
                        {
                            if (partGraph.Vertices.Contains(edge.Target))
                            {
                                if (!partGraph.HasEdge(src.Key, edge.Target.Key))
                                {
                                    partGraph.AddEdge(src.Key, edge.Target.Key);
                                }

                                src = edge.Target;
                            }
                        }
                    }
                }
            }

            return partGraph;
        }

        public async Task<BnGraph> GenerateAsync(BnGraph srcGraph, string group)
        {
            return await Task.Run(() => this.Generate(srcGraph, group));
        }
    }
}