using Marv.Common;
using QuickGraph;
using Telerik.Windows.Diagrams.Core;

namespace Marv
{
    public class Edge : NotifyPropertyChanged, IEdge<Vertex>, ILink<Vertex>
    {
        private EdgeConnectorPositions connectorPositions = new EdgeConnectorPositions();
        private Vertex source;
        private Vertex target;

        public EdgeConnectorPositions ConnectorPositions
        {
            get { return this.connectorPositions; }

            set
            {
                if (value != this.connectorPositions)
                {
                    this.connectorPositions = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public Vertex Source
        {
            get { return this.source; }

            set
            {
                if (value != this.source)
                {
                    this.source = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        object ILink.Source
        {
            get { return this.Source; }
            set { this.Source = value as Vertex; }
        }

        public Vertex Target
        {
            get { return this.target; }

            set
            {
                if (value != this.target)
                {
                    this.target = value;
                    this.RaisePropertyChanged("Target");
                }
            }
        }

        object ILink.Target
        {
            get { return this.Target; }
            set { this.Target = value as Vertex; }
        }

        public Edge(Vertex source, Vertex target)
        {
            this.Source = source;
            this.Target = target;
        }

        public override string ToString()
        {
            var str = "";

            if (this.Source == null)
            {
                str += "null";
            }
            else
            {
                str += this.Source.Key;
            }

            str += " -> ";

            if (this.Target == null)
            {
                str += "null";
            }
            else
            {
                str += this.Target.Key;
            }

            return base.ToString() + ": " + str;
        }
    }
}