using System.Collections.Generic;
using System.Linq;
using QuickGraph;

namespace Marv
{
    public partial class Graph : IBidirectionalGraph<Vertex, Edge>
    {
        public bool AllowParallelEdges
        {
            get { return false; }
        }

        public int EdgeCount
        {
            get { return this.Edges.Count; }
        }

        IEnumerable<Edge> IEdgeSet<Vertex, Edge>.Edges
        {
            get { return this.Edges; }
        }

        public bool IsDirected
        {
            get { return true; }
        }

        public bool IsEdgesEmpty
        {
            get
            {
                if (this.EdgeCount == 0)
                {
                    return true;
                }
                return false;
            }
        }

        public bool IsVerticesEmpty
        {
            get
            {
                if (this.VertexCount == 0)
                {
                    return true;
                }
                return false;
            }
        }

        public int VertexCount
        {
            get { return this.Vertices.Count; }
        }

        IEnumerable<Vertex> IVertexSet<Vertex>.Vertices
        {
            get { return this.Vertices; }
        }

        public bool ContainsEdge(Vertex source, Vertex target)
        {
            return this.Edges.Any(edge => edge.Source == source && edge.Target == target);
        }

        public bool ContainsEdge(Edge edge)
        {
            return this.Edges.Contains(edge);
        }

        public bool ContainsVertex(Vertex vertex)
        {
            return this.Vertices.Contains(vertex);
        }

        public int Degree(Vertex vertex)
        {
            return this.Edges.Count(edge => edge.Source == vertex || edge.Target == vertex);
        }

        public int InDegree(Vertex vertex)
        {
            return this.Edges.Count(edge => edge.Target == vertex);
        }

        public Edge InEdge(Vertex vertex, int index)
        {
            return this.Edges.Where(edge => edge.Target == vertex).ElementAt(index);
        }

        public IEnumerable<Edge> InEdges(Vertex vertex)
        {
            return this.Edges.Where(edge => edge.Target == vertex);
        }

        public bool IsInEdgesEmpty(Vertex vertex)
        {
            return !this.InEdges(vertex).Any();
        }

        public bool IsOutEdgesEmpty(Vertex vertex)
        {
            return this.OutDegree(vertex) == 0;
        }

        public int OutDegree(Vertex vertex)
        {
            return this.Edges.Count(edge => edge.Source == vertex);
        }

        public Edge OutEdge(Vertex vertex, int index)
        {
            return this.OutEdges(vertex).ElementAt(index);
        }

        public IEnumerable<Edge> OutEdges(Vertex vertex)
        {
            return this.Edges.Where(edge => edge.Source == vertex);
        }

        public bool TryGetEdge(Vertex source, Vertex target, out Edge outEdge)
        {
            foreach (var edge in this.Edges)
            {
                if (edge.Source == source && edge.Target == target)
                {
                    outEdge = edge;
                    return true;
                }
            }

            outEdge = null;
            return false;
        }

        public bool TryGetEdges(Vertex source, Vertex target, out IEnumerable<Edge> outEdges)
        {
            var foundEdges = this.Edges.Where(edge => edge.Source == source && edge.Target == target).ToList();

            if (foundEdges.Count > 0)
            {
                outEdges = foundEdges;
                return true;
            }

            outEdges = null;
            return false;
        }

        public bool TryGetInEdges(Vertex vertex, out IEnumerable<Edge> edges)
        {
            return (edges = this.InEdges(vertex)).Any();
        }

        public bool TryGetOutEdges(Vertex vertex, out IEnumerable<Edge> outEdges)
        {
            return (outEdges = this.Edges.Where(edge => edge.Source == vertex)).Any();
        }
    }
}