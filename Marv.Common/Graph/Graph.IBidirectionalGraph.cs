using QuickGraph;
using System.Collections.Generic;
using System.Linq;

namespace Marv.Common
{
    public partial class Graph : IBidirectionalGraph<Vertex, Edge>
    {
        public bool AllowParallelEdges
        {
            get
            {
                return false;
            }
        }

        public int EdgeCount
        {
            get
            {
                return this.Edges.Count;
            }
        }

        IEnumerable<Edge> IEdgeSet<Vertex, Edge>.Edges
        {
            get
            {
                return this.Edges;
            }
        }

        public bool IsDirected
        {
            get
            {
                return true;
            }
        }

        public bool IsEdgesEmpty
        {
            get
            {
                if (this.EdgeCount == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
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
                else
                {
                    return false;
                }
            }
        }

        IEnumerable<Vertex> IVertexSet<Vertex>.Vertices
        {
            get
            {
                return this.Vertices;
            }
        }

        public int VertexCount
        {
            get
            {
                return this.Vertices.Count;
            }
        }

        public bool ContainsEdge(Vertex source, Vertex target)
        {
            foreach (var edge in this.Edges)
            {
                if (edge.Source == source && edge.Target == target)
                {
                    return true;
                }
            }

            return false;
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
            return this.Edges.Where(edge => edge.Source == vertex || edge.Target == vertex).Count();
        }

        public int InDegree(Vertex vertex)
        {
            return this.Edges.Where(edge => edge.Target == vertex).Count();
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
            if (this.InEdges(vertex).Count() == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsOutEdgesEmpty(Vertex vertex)
        {
            if (this.OutDegree(vertex) > 0)
            {
                return true;
            }

            return false;
        }

        public int OutDegree(Vertex vertex)
        {
            var outDegree = 0;

            foreach (var edge in this.Edges)
            {
                if (edge.Source == vertex)
                {
                    outDegree++;
                }
            }

            return outDegree;
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
            var foundEdges = new List<Edge>();

            foreach (var edge in this.Edges)
            {
                if (edge.Source == source && edge.Target == target)
                {
                    foundEdges.Add(edge);
                }
            }

            if (foundEdges.Count > 0)
            {
                outEdges = foundEdges;
                return true;
            }
            else
            {
                outEdges = null;
                return false;
            }
        }

        public bool TryGetInEdges(Vertex vertex, out IEnumerable<Edge> edges)
        {
            edges = this.InEdges(vertex);

            if (edges.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryGetOutEdges(Vertex vertex, out IEnumerable<Edge> outEdges)
        {
            outEdges = this.Edges.Where(edge => edge.Source == vertex);

            if (outEdges.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}