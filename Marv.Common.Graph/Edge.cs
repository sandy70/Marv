using NLog;
using QuickGraph;
using Telerik.Windows.Diagrams.Core;

namespace Marv.Common.Graph
{
    public class Edge : Model, IEdge<Vertex>, ILink<Vertex>
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private EdgeConnectorPositions connectorPositions = new EdgeConnectorPositions();
        private Vertex source;
        private Vertex target;

        public Edge(Vertex source, Vertex target)
        {
            this.Source = source;
            this.Target = target;
        }

        public EdgeConnectorPositions ConnectorPositions
        {
            get
            {
                return this.connectorPositions;
            }

            set
            {
                if (value != this.connectorPositions)
                {
                    this.connectorPositions = value;
                    this.RaisePropertyChanged("ConnectorPositions");
                }
            }
        }

        object ILink.Source
        {
            get
            {
                return this.Source;
            }
            set
            {
                this.Source = value as Vertex;
            }
        }

        object ILink.Target
        {
            get
            {
                return this.Target;
            }
            set
            {
                this.Target = value as Vertex;
            }
        }

        public Vertex Source
        {
            get
            {
                return this.source;
            }

            set
            {
                if (value != this.source)
                {
                    this.source = value;
                    this.RaisePropertyChanged("Source");
                }
            }
        }

        public Vertex Target
        {
            get
            {
                return this.target;
            }

            set
            {
                if (value != this.target)
                {
                    this.target = value;
                    this.RaisePropertyChanged("Target");
                }
            }
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