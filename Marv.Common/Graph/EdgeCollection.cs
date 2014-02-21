using System.Collections.Generic;
using System.Linq;

namespace Marv.Common
{
    public class EdgeCollection : ModelCollection<Edge>
    {
        public void Add(Vertex source, Vertex target)
        {
            this.Add(new Edge(source, target));
        }

        public bool AddUnique(Vertex source, Vertex target)
        {
            return this.AddUnique(source, target);
        }

        public bool AddUnique(Vertex source, Vertex target, EdgeConnectorPositions connectorPostions = null)
        {
            var newEdge = new Edge(source, target);
            newEdge.ConnectorPositions = connectorPostions ?? new EdgeConnectorPositions();

            if (this.Contains(source, target))
            {
                return false;
            }
            else
            {
                this.Add(newEdge);
                return true;
            }
        }

        public bool Contains(Vertex source, Vertex target)
        {
            foreach (var edge in this)
            {
                if (edge.Source == source && edge.Target == target)
                {
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<Edge> GetOutEdges(Vertex vertex)
        {
            return this.Where(edge => edge.Source == vertex);
        }
    }
}