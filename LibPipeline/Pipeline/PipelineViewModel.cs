using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace LibPipeline
{
    public class PipelineViewModel : INotifyPropertyChanged
    {
        public static string SelectedColor = "DarkGoldenrod";
        public static string UnselectedColor = "Brown";

        private string color;
        private LandcoverViewModel landcoverViewModel;
        private ObservableCollection<Location> locations;
        private MultiLocation pipeline;
        private ObservableCollection<Point> riverCrossingPoints;
        private ObservableCollection<Point> roadCrossingPoints;
        private PipelineSegmentInstant selectedInstant;
        private int selectedInstantIndex;
        private Location selectedLocation;
        private LocationCollection selectedSegmentPolyline;
        private Visibility selectedSegmentVisibility;
        private LocationCollection simplifiedLocations;
        private ObservableCollection<Percentile> tpdPercentiles;
        private Visibility visibility;
        private ObservableCollection<Percentile> wallLossPercentiles;

        public PipelineViewModel()
        {
            this.Color = PipelineViewModel.UnselectedColor;
            this.LandcoverViewModel = new LandcoverViewModel();
            this.Locations = new ObservableCollection<Location>();
            this.Pipeline = new MultiLocation();
            this.RiverCrossingPoints = new ObservableCollection<Point>();
            this.RoadCrossingPoints = new ObservableCollection<Point>();
            this.SelectedInstant = new PipelineSegmentInstant();
            this.SelectedInstantIndex = 0;
            this.SelectedLocation = new Location();
            this.SelectedSegmentVisibility = Visibility.Collapsed;
            this.SelectedSegmentPolyline = new LocationCollection();
            this.SimplifiedLocations = new LocationCollection();
            this.TpdPercentiles = new ObservableCollection<Percentile>();
            this.Visibility = Visibility.Visible;
            this.WallLossPercentiles = new ObservableCollection<Percentile>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Color
        {
            get { return color; }
            set
            {
                color = value;
                OnPropertyChanged("Color");
            }
        }

        public LandcoverViewModel LandcoverViewModel
        {
            get { return landcoverViewModel; }
            set
            {
                landcoverViewModel = value;
                OnPropertyChanged("LandcoverViewModel");
            }
        }

        public ObservableCollection<Location> Locations
        {
            get { return locations; }
            set
            {
                locations = value;
                OnPropertyChanged("Locations");
            }
        }

        public MultiLocation Pipeline
        {
            get { return pipeline; }
            set
            {
                pipeline = value;
                OnPropertyChanged("MultiLocation");
            }
        }

        public ObservableCollection<Point> RiverCrossingPoints
        {
            get { return riverCrossingPoints; }
            set
            {
                riverCrossingPoints = value;
                OnPropertyChanged("RiverCrossingPoints");
            }
        }

        public ObservableCollection<Point> RoadCrossingPoints
        {
            get { return roadCrossingPoints; }
            set
            {
                roadCrossingPoints = value;
                OnPropertyChanged("RoadCrossingPoints");
            }
        }

        public PipelineSegmentInstant SelectedInstant
        {
            get { return selectedInstant; }
            set
            {
                selectedInstant = value;
                OnPropertyChanged("SelectedInstant");
            }
        }

        public int SelectedInstantIndex
        {
            get { return selectedInstantIndex; }
            set
            {
                selectedInstantIndex = value;
                OnPropertyChanged("SelectedInstantIndex");
            }
        }

        public Location SelectedLocation
        {
            get { return selectedLocation; }
            set
            {
                selectedLocation = value;
                OnPropertyChanged("SelectedLocation");
            }
        }

        public LocationCollection SelectedSegmentPolyline
        {
            get { return selectedSegmentPolyline; }
            set
            {
                selectedSegmentPolyline = value;
                OnPropertyChanged("SelectedSegmentPolyline");
            }
        }

        public Visibility SelectedSegmentVisibility
        {
            get { return selectedSegmentVisibility; }
            set
            {
                selectedSegmentVisibility = value;
                OnPropertyChanged("SelectedSegmentVisibility");
            }
        }

        public LocationCollection SimplifiedLocations
        {
            get { return simplifiedLocations; }
            set
            {
                simplifiedLocations = value;
                OnPropertyChanged("SimplifiedLocations");
            }
        }

        public ObservableCollection<Percentile> TpdPercentiles
        {
            get { return tpdPercentiles; }
            set
            {
                tpdPercentiles = value;
                OnPropertyChanged("TpdPercentiles");
            }
        }

        public Visibility Visibility
        {
            get { return visibility; }
            set
            {
                visibility = value;
                OnPropertyChanged("Visibility");
            }
        }

        public ObservableCollection<Percentile> WallLossPercentiles
        {
            get { return wallLossPercentiles; }
            set
            {
                wallLossPercentiles = value;
                OnPropertyChanged("WallLossPercentiles");
            }
        }

        public void Clear()
        {
            // do nothing
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}