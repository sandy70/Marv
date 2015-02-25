using System.Collections;
using System.Collections.Generic;
using Marv.Common;
using Telerik.Windows.Diagrams.Core;

namespace Marv
{
    public partial class Graph : IObservableGraphSource
    {
        public IEnumerable Items
        {
            get
            {
                return this.Vertices;
            }
        }

        public IEnumerable<ILink> Links
        {
            get
            {
                return this.Edges;
            }
        }

        public void AddLink(ILink link)
        {
            this.Edges.Add(link as Edge);
        }

        public void AddNode(object node)
        {
            this.Vertices.Add(node as Vertex);
        }

        public ILink CreateLink(object source, object target)
        {
            return new Edge(source as Vertex, target as Vertex);
        }

        public object CreateNode(IShape shape)
        {
            return new Vertex();
        }

        public bool RemoveLink(ILink link)
        {
            return this.Edges.Remove(link as Edge);
        }

        public bool RemoveNode(object node)
        {
            return this.Vertices.Remove(node as Vertex);
        }
    }
}