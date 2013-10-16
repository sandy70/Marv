using Marv.Common;
using Marv.Common;
using System.Windows;

namespace Marv.Controls
{
    public class BnGraphControlEventArgs : RoutedEventArgs
    {
        private State state;

        private Vertex vertex;

        public BnGraphControlEventArgs()
            : base()
        {
        }

        public BnGraphControlEventArgs(RoutedEvent routedEvent)
            : base(routedEvent)
        {
        }

        public BnGraphControlEventArgs(RoutedEvent routedEvent, object source)
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