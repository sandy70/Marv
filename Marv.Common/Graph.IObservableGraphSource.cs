using System.Collections;
using System.Collections.Generic;
using Telerik.Windows.Diagrams.Core;

namespace Marv.Common
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
                return this.edges;
            }
        }

        public void AddLink(ILink link)
        {
            this.edges.Add(link as Edge);
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
            return this.edges.Remove(link as Edge);
        }

        public bool RemoveNode(object node)
        {
            return this.Vertices.Remove(node as Vertex);
        }
    }
}