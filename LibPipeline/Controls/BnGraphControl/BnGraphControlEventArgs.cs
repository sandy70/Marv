using LibNetwork;
using System.Windows;

namespace LibPipeline
{
    public class BnGraphControlEventArgs : RoutedEventArgs
    {
        private BnState state;

        private BnVertexViewModel vertex;

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

        public BnState State
        {
            get { return state; }
            set { state = value; }
        }

        public BnVertexViewModel Vertex
        {
            get { return vertex; }
            set { vertex = value; }
        }
    }
}