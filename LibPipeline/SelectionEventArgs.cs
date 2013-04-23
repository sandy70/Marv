using System.Windows;

namespace LibPipeline
{
    public class ValueEventArgs<T> : RoutedEventArgs
    {
        public ValueEventArgs()
            : base()
        {
        }

        public ValueEventArgs(RoutedEvent routedEvent)
            : base(routedEvent)
        {
        }

        public ValueEventArgs(RoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        {
        }

        public T Value { get; set; }
    }
}