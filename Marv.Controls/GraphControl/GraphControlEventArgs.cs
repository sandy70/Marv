using Marv.Common;
using System.Windows;

namespace Marv.Controls
{
    public class GraphControlEventArgs : RoutedEventArgs
    {
        private State state;

        private Vertex vertex;

        public GraphControlEventArgs()
            : base()
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
            get { return state; }
            set { state = value; }
        }

        public Vertex Vertex
        {
            get { return vertex; }
            set { vertex = value; }
        }
    }
}