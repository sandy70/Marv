using System.Collections.Generic;
using System.Linq;

namespace Marv.Common.Graph
{
    public static partial class Extensions
    {
        public static void AddUnique(this ModelCollection<Edge> edges, Vertex source, Vertex target, EdgeConnectorPositions connectorPostions = null)
        {
            var newEdge = new Edge(source, target)
            {
                ConnectorPositions = connectorPostions ?? new EdgeConnectorPositions()
            };

            if (Contains(edges, source, target))
            {
                return;
            }

            edges.Add(newEdge);
        }

        public static bool Contains(this IEnumerable<Edge> edges, Vertex source, Vertex target)
        {
            return edges.Any(edge => edge.Source == source && edge.Target == target);
        }

        public static Dictionary<string, double> Parse(this IEnumerable<State> states, IDistribution dist)
        {
            var evidence = new Dictionary<string, double>();

            foreach (var state in states)
            {
                // if max is inf then use min * 2;
                var max = state.Max == double.PositiveInfinity ? state.Min * 2 : state.Max;

                evidence[state.Key] = dist.Cdf(max) - dist.Cdf(state.Min);
            }

            return evidence.Normalized();
        }
    }
}