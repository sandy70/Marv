using System.Collections.Generic;
using System.Linq;

namespace Marv
{
    public static partial class Extensions
    {
        public static void AddUnique(this ICollection<Edge> edges, Vertex source, Vertex target, EdgeConnectorPositions connectorPostions = null)
        {
            var newEdge = new Edge(source, target)
            {
                ConnectorPositions = connectorPostions ?? new EdgeConnectorPositions()
            };

            if (edges.Contains(source, target))
            {
                return;
            }

            edges.Add(newEdge);
        }

        public static bool Contains(this IEnumerable<Edge> edges, Vertex source, Vertex target)
        {
            return edges.Any(edge => edge.Source == source && edge.Target == target);
        }
    }
}