using System;
using System.ComponentModel;
using Telerik.Windows.Diagrams.Core;

namespace Marv.Common
{
    [Serializable]
    public class Edge : QuickGraph.Edge<Vertex>, ILink<Vertex>, INotifyPropertyChanged
    {
        private double _value = 1;
        private string sourceConnectorPosition = "Auto";
        private string targetConnectorPosition = "Auto";

        public Edge(Vertex source, Vertex target)
            : base(source, target)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        object ILink.Source
        {
            get
            {
                return this.Source;
            }
            set
            {
                object temp = value;
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
                object temp = value;
            }
        }

        public new Vertex Source
        {
            get { return base.Source; }
            set { }
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

        public new Vertex Target
        {
            get { return base.Target; }
            set { }
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

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}