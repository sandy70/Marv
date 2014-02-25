using System.Windows;
using Marv.Common.Graph;

namespace Marv.Controls.Graph
{
    public class GraphControlEventArgs : RoutedEventArgs
    {
        private State state;

        private Vertex vertex;

        public GraphControlEventArgs()
        {
        }

        public GraphControlEventArgs(RoutedEvent routedEvent)
            : base(routedEvent)
        {
        }

        public GraphControlEventArgs(RoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        {
        }

        public State State
        {
            get { return this.state; }
            set { this.state = value; }
        }

        public Vertex Vertex
        {
            get { return this.vertex; }
            set { this.vertex = value; }
        }
    }
}