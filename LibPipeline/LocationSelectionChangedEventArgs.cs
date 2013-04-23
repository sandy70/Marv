using System.Windows;

namespace LibPipeline
{
    public class LocationSelectionChangedEventArgs : RoutedEventArgs
    {
        public LocationSelectionChangedEventArgs()
            : base()
        {
        }

        public LocationSelectionChangedEventArgs(RoutedEvent routedEvent)
            : base(routedEvent)
        {
        }

        public LocationSelectionChangedEventArgs(RoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        {
        }

        public ILocation NewLocation { get; set; }
    }
}