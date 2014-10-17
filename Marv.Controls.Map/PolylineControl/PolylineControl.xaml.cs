using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Marv.Map;

namespace Marv.Controls.Map
{
    public partial class PolylineControl : INotifyPropertyChanged
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

        public static readonly DependencyProperty SelectedLocationProperty =
            DependencyProperty.Register("SelectedLocation", typeof (Location), typeof (PolylineControl), new PropertyMetadata(null));

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof (Brush), typeof (PolylineControl), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof (double), typeof (PolylineControl), new PropertyMetadata(3.0));

        private IEnumerable<Location> simplifiedLocations;

        public Brush CursorFill
        {
            get
            {
                return (Brush) this.GetValue(CursorFillProperty);
            }
            set
            {
                this.SetValue(CursorFillProperty, value);
            }
        }

        public Location CursorLocation
        {
            get
            {
                return (Location) this.GetValue(CursorLocationProperty);
            }
            set
            {
                this.SetValue(CursorLocationProperty, value);
            }
        }

        public Brush CursorStroke
        {
            get
            {
                return (Brush) this.GetValue(CursorStrokeProperty);
            }
            set
            {
                this.SetValue(CursorStrokeProperty, value);
            }
        }

        public bool IsCursorVisible
        {
            get
            {
                return (bool) this.GetValue(IsCursorVisibleProperty);
            }
            set
            {
                this.SetValue(IsCursorVisibleProperty, value);
            }
        }

        public IEnumerable<Location> Locations
        {
            get
            {
                return (LocationCollection) this.GetValue(LocationsProperty);
            }
            set
            {
                this.SetValue(LocationsProperty, value);
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
                return this.simplifiedLocations;
            }

            set
            {
                if (value.Equals(this.simplifiedLocations))
                {
                    return;
                }

                this.simplifiedLocations = value;
                this.RaisePropertyChanged();
            }
        }

        public Brush Stroke
        {
            get
            {
                return (Brush) this.GetValue(StrokeProperty);
            }
            set
            {
                this.SetValue(StrokeProperty, value);
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

        public PolylineControl()
        {
            this.InitializeComponent();
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
                var firstLocation = control.Locations.First();

                control.CursorLocation = firstLocation;
                control.UpdateSimplifiedLocations();
                control.RaiseSelectionChanged(firstLocation);
            }
        }

        public void RaiseSelectionChanged(Location location)
        {
            this.SelectedLocation = location;

            if (this.SelectionChanged != null)
            {
                this.SelectionChanged(this, location);
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

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null && propertyName != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<Location> SelectionChanged;
    }
}