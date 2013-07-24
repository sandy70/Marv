using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LibPipeline
{
    public partial class SegmentedPolylineControl : UserControl
    {
        public static readonly DependencyProperty LocationsProperty =
        DependencyProperty.Register("Locations", typeof(IEnumerable<Location>), typeof(SegmentedPolylineControl), new PropertyMetadata(null, ChangedLocations));

        public static readonly DependencyProperty SegmentsProperty =
        DependencyProperty.Register("Segments", typeof(ObservableCollection<MultiLocationSegment>), typeof(SegmentedPolylineControl), new PropertyMetadata(new ObservableCollection<MultiLocationSegment>()));

        public SegmentedPolylineControl()
        {
            InitializeComponent();
        }

        public IEnumerable<Location> Locations
        {
            get { return (IEnumerable<Location>)GetValue(LocationsProperty); }
            set { SetValue(LocationsProperty, value); }
        }

        public ObservableCollection<MultiLocationSegment> Segments
        {
            get { return (ObservableCollection<MultiLocationSegment>)GetValue(SegmentsProperty); }
            set { SetValue(SegmentsProperty, value); }
        }

        private static void ChangedLocations(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SegmentedPolylineControl;

            // var locations = control.Locations.Take(100);
            var locations = control.Locations;

            Location start = null;
            Location middle = null;
            Location end = null;

            var index = 0;

            var segments = new ObservableCollection<MultiLocationSegment>();

            foreach (var location in locations)
            {
                start = middle;
                middle = end;
                end = location;

                if (index == 1)
                {
                    segments.Add(new MultiLocationSegment
                    {
                        Middle = middle,
                        End = end
                    });
                }
                else
                {
                    segments.Add(new MultiLocationSegment
                    {
                        Start = start,
                        Middle = middle,
                        End = end
                    });

                    if (index == locations.Count() - 1)
                    {
                        segments.Add(new MultiLocationSegment
                        {
                            Start = middle,
                            Middle = end
                        });
                    }
                }

                index++;
            }

            control.Segments = segments;
        }
    }
}