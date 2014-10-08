﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Marv.Map;

namespace Marv.Controls.Map
{
    public class PolylineControlBase : UserControl
    {
        public static readonly DependencyProperty CursorFillProperty =
            DependencyProperty.Register("CursorFill", typeof (Brush), typeof (PolylineControlBase), new PropertyMetadata(new SolidColorBrush(Colors.YellowGreen)));

        public static readonly DependencyProperty CursorLocationProperty =
            DependencyProperty.Register("CursorLocation", typeof (Location), typeof (PolylineControlBase), new PropertyMetadata(null));

        public static readonly DependencyProperty CursorStrokeProperty =
            DependencyProperty.Register("CursorStroke", typeof (Brush), typeof (PolylineControlBase), new PropertyMetadata(new SolidColorBrush(Colors.Yellow)));

        public static readonly DependencyProperty IsCursorVisibleProperty =
            DependencyProperty.Register("IsCursorVisible", typeof (bool), typeof (PolylineControlBase), new PropertyMetadata(true));

        public static readonly DependencyProperty LocationsProperty =
            DependencyProperty.Register("Locations", typeof (LocationCollection), typeof (PolylineControlBase), new PropertyMetadata(null, ChangedLocations));

        public static readonly DependencyProperty SelectedLocationProperty =
            DependencyProperty.Register("SelectedLocation", typeof (Location), typeof (PolylineControlBase), new PropertyMetadata(null, OnSelectedLocationChanged));

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof (Brush), typeof (PolylineControlBase), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof (double), typeof (PolylineControl), new PropertyMetadata(5.0));

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

        public LocationCollection Locations
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
                return (Location) this.GetValue(SelectedLocationProperty);
            }
            set
            {
                this.SetValue(SelectedLocationProperty, value);
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
                return (double) this.GetValue(StrokeThicknessProperty);
            }
            set
            {
                this.SetValue(StrokeThicknessProperty, value);
            }
        }

        // Virtual function will be overridden in derived classes
        // Do not remove

        private static void ChangedLocations(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as PolylineControlBase;

            if (control != null)
            {
                control.OnChangedLocations();
            }
        }

        private static void OnSelectedLocationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as PolylineControlBase;

            if (control != null)
            {
                control.CursorLocation = control.SelectedLocation;
                control.IsCursorVisible = true;
            }
        }

        protected virtual void OnChangedLocations() {}
    }
}