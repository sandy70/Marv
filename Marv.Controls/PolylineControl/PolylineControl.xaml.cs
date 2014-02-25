using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Marv.Common;
using Marv.Common.Map;

namespace Marv.Controls
{
    public partial class PolylineControl
    {
        public static readonly DependencyProperty CursorFillProperty =
            DependencyProperty.Register("CursorFill", typeof (Brush), typeof (PolylineControl), new PropertyMetadata(new SolidColorBrush(Colors.YellowGreen)));

        public static readonly DependencyProperty CursorLocationProperty =
            DependencyProperty.Register("CursorLocation", typeof (Location), typeof (PolylineControl), new PropertyMetadata(null, ChangedCursorLocation));

        public static readonly DependencyProperty CursorStrokeProperty =
            DependencyProperty.Register("CursorStroke", typeof (Brush), typeof (PolylineControl), new PropertyMetadata(new SolidColorBrush(Colors.Yellow)));

        public static readonly DependencyProperty IsCursorVisibleProperty =
            DependencyProperty.Register("IsCursorVisible", typeof (bool), typeof (PolylineControl), new PropertyMetadata(false));

        public static readonly DependencyProperty LocationsProperty =
            DependencyProperty.Register("Locations", typeof (IEnumerable<Location>), typeof (PolylineControl), new PropertyMetadata(null, ChangedLocations));

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof (Brush), typeof (PolylineControl), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        public static readonly DependencyProperty SimplifiedLocationsProperty =
            DependencyProperty.Register("SimplifiedLocations", typeof (IEnumerable<Location>), typeof (PolylineControl), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedLocationProperty =
            DependencyProperty.Register("SelectedLocation", typeof (Location), typeof (PolylineControl), new PropertyMetadata(null, ChangedSelectedLocation));

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof (double), typeof (PolylineControl), new PropertyMetadata(5.0));

        public static readonly RoutedEvent SelectionChangedEvent =
            EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Bubble, typeof (RoutedEventHandler<ValueEventArgs<Location>>), typeof (PolylineControl));

        public PolylineControl()
        {
            this.InitializeComponent();
        }

        public Brush CursorFill
        {
            get
            {
                return (Brush) GetValue(CursorFillProperty);
            }
            set
            {
                SetValue(CursorFillProperty, value);
            }
        }

        public Location CursorLocation
        {
            get
            {
                return (Location) GetValue(CursorLocationProperty);
            }
            set
            {
                SetValue(CursorLocationProperty, value);
            }
        }

        public Brush CursorStroke
        {
            get
            {
                return (Brush) GetValue(CursorStrokeProperty);
            }
            set
            {
                SetValue(CursorStrokeProperty, value);
            }
        }

        public bool IsCursorVisible
        {
            get
            {
                return (bool) GetValue(IsCursorVisibleProperty);
            }
            set
            {
                SetValue(IsCursorVisibleProperty, value);
            }
        }

        public IEnumerable<Location> Locations
        {
            get
            {
                return (LocationCollection) GetValue(LocationsProperty);
            }
            set
            {
                SetValue(LocationsProperty, value);
            }
        }

        public Location SelectedLocation
        {
            get
            {
                return (Location) GetValue(SelectedLocationProperty);
            }
            set
            {
                SetValue(SelectedLocationProperty, value);
            }
        }

        public IEnumerable<Location> SimplifiedLocations
        {
            get
            {
                return (IEnumerable<Location>) this.GetValue(SimplifiedLocationsProperty);
            }
            set
            {
                this.SetValue(SimplifiedLocationsProperty, value);
            }
        }

        public Brush Stroke
        {
            get
            {
                return (Brush) GetValue(StrokeProperty);
            }
            set
            {
                SetValue(StrokeProperty, value);
            }
        }

        public double StrokeThickness
        {
            get
            {
                return (double) GetValue(StrokeThicknessProperty);
            }
            set
            {
                SetValue(StrokeThicknessProperty, value);
            }
        }

        private static void ChangedCursorLocation(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as PolylineControl;

            if (control != null)
            {
                control.IsCursorVisible = control.CursorLocation != null;
            }
        }

        private static void ChangedLocations(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as PolylineControl;

            if (control != null && control.Locations != null)
            {
                control.CursorLocation = control.Locations.FirstOrDefault();
                control.UpdateSimplifiedLocations();
            }
        }

        private static void ChangedSelectedLocation(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as PolylineControl;

            if (control != null)
            {
                control.RaiseSelectionChanged();
            }
        }

        public void RaiseSelectionChanged()
        {
            if (this.SelectedLocation != null)
            {
                this.RaiseEvent(new ValueEventArgs<Location>
                {
                    RoutedEvent = SelectionChangedEvent,
                    Value = this.SelectedLocation
                });
            }
        }

        public void UpdateSimplifiedLocations()
        {
            var mapView = this.FindParent<MapView>();

            this.SimplifiedLocations = this.Locations
                .ToPoints(mapView)
                .Reduce(5)
                .ToLocations(mapView);
        }

        public event RoutedEventHandler<ValueEventArgs<Location>> SelectionChanged
        {
            add
            {
                AddHandler(SelectionChangedEvent, value);
            }
            remove
            {
                RemoveHandler(SelectionChangedEvent, value);
            }
        }
    }
}