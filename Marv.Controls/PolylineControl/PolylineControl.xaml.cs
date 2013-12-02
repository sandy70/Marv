using Marv.Common;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Marv.Controls
{
    public partial class PolylineControl : PolylineControlBase
    {
        public static readonly RoutedEvent SelectionChangedEvent =
        EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler<ValueEventArgs<Location>>), typeof(PolylineControl));

        public static readonly DependencyProperty SimplifiedLocationsProperty =
        DependencyProperty.Register("SimplifiedLocations", typeof(IEnumerable<Location>), typeof(PolylineControl), new PropertyMetadata(null));

        public PolylineControl()
        {
            InitializeComponent();
        }

        public IEnumerable<Location> SimplifiedLocations
        {
            get { return (IEnumerable<Location>)GetValue(SimplifiedLocationsProperty); }
            set { SetValue(SimplifiedLocationsProperty, value); }
        }

        protected override void OnChangedLocations()
        {
            this.SimplifiedLocations = this.Locations;
            this.SelectedLocation = this.Locations.FirstOrDefault();
            // this.CursorLocation = this.SelectedLocation;
        }
    }
}