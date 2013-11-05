using QuickGraph;
using System;
using System.ComponentModel;
using Telerik.Windows.Diagrams.Core;

namespace Marv.Common
{
    public class Edge : ViewModel, IEdge<Vertex>, ILink<Vertex>
    {
        private double _value = 1;
        private Vertex source;
        private string sourceConnectorPosition = "Auto";
        private Vertex target;
        private string targetConnectorPosition = "Auto";

        public Edge(Vertex source, Vertex target)
        {
            this.Source = source;
            this.Target = target;
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

        public string SourceConnectorPosition
        {
            get
            {
                return this.sourceConnectorPosition;
            }

            set
            {
                if (value != this.sourceConnectorPosition)
                {
                    this.sourceConnectorPosition = value;
                    this.RaisePropertyChanged("SourceConnectorPosition");
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

        public string TargetConnectorPosition
        {
            get
            {
                return this.targetConnectorPosition;
            }

            set
            {
                if (value != this.targetConnectorPosition)
                {
                    this.targetConnectorPosition = value;
                    this.RaisePropertyChanged("TargetConnectorPosition");
                }
            }
        }

        public double Value
        {
            get
            {
                return this._value;
            }

            set
            {
                if (value != this._value)
                {
                    this._value = value;
                    this.RaisePropertyChanged("Values");
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
    }
}