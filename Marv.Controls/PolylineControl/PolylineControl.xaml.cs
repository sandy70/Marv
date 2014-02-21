using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Marv.Common.Map;

namespace Marv.Controls
{
    public partial class PolylineControl
    {
        public event RoutedEventHandler<ValueEventArgs<Location>> SelectionChanged
		{
			add { this.AddHandler(SelectionChangedEvent, value); }
			remove { this.RemoveHandler(SelectionChangedEvent, value); }
		}

		public static readonly RoutedEvent SelectionChangedEvent =
		EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler<ValueEventArgs<Location>>), typeof(PolylineControl));

        public static readonly DependencyProperty SimplifiedLocationsProperty =
        DependencyProperty.Register("SimplifiedLocations", typeof(IEnumerable<Location>), typeof(PolylineControl), new PropertyMetadata(null));

        public PolylineControl()
        {
            this.InitializeComponent();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property.Name == "SelectedLocation")
            {
                this.RaiseSelectionChanged(this.SelectedLocation);
            }
        }

        public IEnumerable<Location> SimplifiedLocations
        {
            get { return (IEnumerable<Location>)this.GetValue(SimplifiedLocationsProperty); }
            set { this.SetValue(SimplifiedLocationsProperty, value); }
        }

        protected override void OnChangedLocations()
        {
            this.SimplifiedLocations = this.Locations;
            this.SelectedLocation = this.Locations.FirstOrDefault();
            // this.CursorLocation = this.SelectedLocation;
        }

        public void RaiseSelectionChanged(Location location)
        {
            this.RaiseEvent(new ValueEventArgs<Location>
            {
                RoutedEvent = PolylineControl.SelectionChangedEvent,
                Value = location
            });
        }
    }
}