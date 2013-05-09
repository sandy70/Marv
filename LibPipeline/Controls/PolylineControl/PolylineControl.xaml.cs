using MapControl;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LibPipeline
{
    public partial class PolylineControl : UserControl
    {
        public static readonly DependencyProperty CursorLocationProperty =
        DependencyProperty.Register("CursorLocation", typeof(ILocation), typeof(PolylineControl), new PropertyMetadata(null));

        public static readonly DependencyProperty IsCursorVisibleProperty =
        DependencyProperty.Register("IsCursorVisible", typeof(bool), typeof(PolylineControl), new PropertyMetadata(true));

        public static readonly DependencyProperty LocationsProperty =
        DependencyProperty.Register("Locations", typeof(IEnumerable<ILocation>), typeof(PolylineControl), new PropertyMetadata(null, ChangedLocations));

        public static readonly DependencyProperty SelectedLocationProperty =
        DependencyProperty.Register("SelectedLocation", typeof(ILocation), typeof(PolylineControl), new PropertyMetadata(null, ChangedLocation));

        public PolylineControl()
        {
            InitializeComponent();
        }

        public ILocation CursorLocation
        {
            get { return (ILocation)GetValue(CursorLocationProperty); }
            set { SetValue(CursorLocationProperty, value); }
        }

        public bool IsCursorVisible
        {
            get { return (bool)GetValue(IsCursorVisibleProperty); }
            set { SetValue(IsCursorVisibleProperty, value); }
        }

        public IEnumerable<ILocation> Locations
        {
            get { return (IEnumerable<ILocation>)GetValue(LocationsProperty); }
            set { SetValue(LocationsProperty, value); }
        }

        public ILocation SelectedLocation
        {
            get { return (ILocation)GetValue(SelectedLocationProperty); }
            set { SetValue(SelectedLocationProperty, value); }
        }

        private static void ChangedLocation(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pipelineControl = d as PolylineControl;
            pipelineControl.CursorLocation = pipelineControl.SelectedLocation;
        }

        private static void ChangedLocations(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pipelineControl = d as PolylineControl;
            pipelineControl.SelectedLocation = (e.NewValue as IEnumerable<ILocation>).FirstOrDefault();
        }
    }
}