using LibPipeline;
using Marv.Common;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Marv
{
    public partial class MainWindow
    {
        public static readonly DependencyProperty CacheDirectoryProperty =
        DependencyProperty.Register("CacheDirectory", typeof(string), typeof(MainWindow), new PropertyMetadata(".\\"));

        public static readonly DependencyProperty ChartPointsProperty =
        DependencyProperty.Register("ChartPoints", typeof(ObservableCollection<ObservableKeyValuePair<int, double>>), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty DisplayGraphProperty =
        DependencyProperty.Register("DisplayGraph", typeof(Graph), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty EndYearProperty =
        DependencyProperty.Register("EndYear", typeof(int), typeof(MainWindow), new PropertyMetadata(2010));

        public static readonly DependencyProperty GraphValueTimeSeriesProperty =
        DependencyProperty.Register("GraphValueTimeSeries", typeof(GraphValueTimeSeries), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty InputFileNameProperty =
        DependencyProperty.Register("InputFileName", typeof(string), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty IsBackButtonVisibleProperty =
        DependencyProperty.Register("IsBackButtonVisible", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty IsGroupButtonVisibleProperty =
        DependencyProperty.Register("IsGroupButtonVisible", typeof(bool), typeof(MainWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty IsLogoVisibleProperty =
        DependencyProperty.Register("IsLogoVisible", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty IsMapVisibleProperty =
        DependencyProperty.Register("IsMapVisible", typeof(bool), typeof(MainWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty IsMenuVisibleProperty =
        DependencyProperty.Register("IsMenuVisible", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty IsProfileSelectedProperty =
        DependencyProperty.Register("IsProfileSelected", typeof(bool), typeof(MainWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty IsPropertyGridVisibleProperty =
        DependencyProperty.Register("IsPropertyGridVisible", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty IsSensorButtonVisibleProperty =
        DependencyProperty.Register("IsSensorButtonVisible", typeof(bool), typeof(MainWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty IsSettingsControlVisibleProperty =
        DependencyProperty.Register("IsSettingsControlVisible", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty IsSettingsVisibleProperty =
        DependencyProperty.Register("IsSettingsVisible", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty IsTallySelectedProperty =
        DependencyProperty.Register("IsTallySelected", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty IsYearSliderVisibleProperty =
        DependencyProperty.Register("IsYearSliderVisible", typeof(bool), typeof(MainWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty MultiLocationsProperty =
        DependencyProperty.Register("MultiLocations", typeof(SelectableCollection<MultiLocation>), typeof(MainWindow), new PropertyMetadata(null, ChangedMultiLocations));

        public static readonly DependencyProperty MultiLocationValueTimeSeriesProperty =
        DependencyProperty.Register("MultiLocationValueTimeSeries", typeof(MultiLocationValueTimeSeries), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty NetworkFileNameProperty =
        DependencyProperty.Register("NetworkFileName", typeof(string), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty NotificationsProperty =
        DependencyProperty.Register("Notifications", typeof(ObservableCollection<INotification>), typeof(MainWindow), new PropertyMetadata(new ObservableCollection<INotification>()));

        public static readonly DependencyProperty RiskValueToBrushMapProperty =
        DependencyProperty.Register("RiskValueToBrushMap", typeof(RiskValueToBrushMap), typeof(MainWindow), new PropertyMetadata(new RiskValueToBrushMap()));

        public static readonly DependencyProperty SelectedYearProperty =
        DependencyProperty.Register("SelectedYear", typeof(int), typeof(MainWindow), new PropertyMetadata(2000, ChangedSelectedYear));

        public static readonly DependencyProperty SourceGraphProperty =
        DependencyProperty.Register("SourceGraph", typeof(Graph), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty StartYearProperty =
        DependencyProperty.Register("StartYear", typeof(int), typeof(MainWindow), new PropertyMetadata(2000));

        public string CacheDirectory
        {
            get { return (string)GetValue(CacheDirectoryProperty); }
            set { SetValue(CacheDirectoryProperty, value); }
        }

        public ObservableCollection<ObservableKeyValuePair<int, double>> ChartPoints
        {
            get { return (ObservableCollection<ObservableKeyValuePair<int, double>>)GetValue(ChartPointsProperty); }
            set { SetValue(ChartPointsProperty, value); }
        }

        public Graph DisplayGraph
        {
            get { return (Graph)GetValue(DisplayGraphProperty); }
            set { SetValue(DisplayGraphProperty, value); }
        }

        public int EndYear
        {
            get { return (int)GetValue(EndYearProperty); }
            set { SetValue(EndYearProperty, value); }
        }

        public GraphValueTimeSeries GraphValueTimeSeries
        {
            get { return (GraphValueTimeSeries)GetValue(GraphValueTimeSeriesProperty); }
            set { SetValue(GraphValueTimeSeriesProperty, value); }
        }

        public string InputFileName
        {
            get { return (string)GetValue(InputFileNameProperty); }
            set { SetValue(InputFileNameProperty, value); }
        }

        public bool IsBackButtonVisible
        {
            get { return (bool)GetValue(IsBackButtonVisibleProperty); }
            set { SetValue(IsBackButtonVisibleProperty, value); }
        }

        public bool IsGroupButtonVisible
        {
            get { return (bool)GetValue(IsGroupButtonVisibleProperty); }
            set { SetValue(IsGroupButtonVisibleProperty, value); }
        }

        public bool IsLogoVisible
        {
            get { return (bool)GetValue(IsLogoVisibleProperty); }
            set { SetValue(IsLogoVisibleProperty, value); }
        }

        public bool IsMapVisible
        {
            get { return (bool)GetValue(IsMapVisibleProperty); }
            set { SetValue(IsMapVisibleProperty, value); }
        }

        public bool IsMenuVisible
        {
            get { return (bool)GetValue(IsMenuVisibleProperty); }
            set { SetValue(IsMenuVisibleProperty, value); }
        }

        public bool IsPropertyGridVisible
        {
            get { return (bool)GetValue(IsPropertyGridVisibleProperty); }
            set { SetValue(IsPropertyGridVisibleProperty, value); }
        }

        public bool IsSensorButtonVisible
        {
            get { return (bool)GetValue(IsSensorButtonVisibleProperty); }
            set { SetValue(IsSensorButtonVisibleProperty, value); }
        }

        public bool IsTallySelected
        {
            get { return (bool)GetValue(IsTallySelectedProperty); }
            set { SetValue(IsTallySelectedProperty, value); }
        }

        public bool IsYearSliderVisible
        {
            get { return (bool)GetValue(IsYearSliderVisibleProperty); }
            set { SetValue(IsYearSliderVisibleProperty, value); }
        }

        public SelectableCollection<MultiLocation> MultiLocations
        {
            get { return (SelectableCollection<MultiLocation>)GetValue(MultiLocationsProperty); }
            set { SetValue(MultiLocationsProperty, value); }
        }

        public MultiLocationValueTimeSeries MultiLocationValueTimeSeries
        {
            get { return (MultiLocationValueTimeSeries)GetValue(MultiLocationValueTimeSeriesProperty); }
            set { SetValue(MultiLocationValueTimeSeriesProperty, value); }
        }

        public string NetworkFileName
        {
            get { return (string)GetValue(NetworkFileNameProperty); }
            set { SetValue(NetworkFileNameProperty, value); }
        }

        public ObservableCollection<INotification> Notifications
        {
            get { return (ObservableCollection<INotification>)GetValue(NotificationsProperty); }
            set { SetValue(NotificationsProperty, value); }
        }

        public RiskValueToBrushMap RiskValueToBrushMap
        {
            get { return (RiskValueToBrushMap)GetValue(RiskValueToBrushMapProperty); }
            set { SetValue(RiskValueToBrushMapProperty, value); }
        }

        public int SelectedYear
        {
            get { return (int)GetValue(SelectedYearProperty); }
            set { SetValue(SelectedYearProperty, value); }
        }

        public Graph SourceGraph
        {
            get { return (Graph)GetValue(SourceGraphProperty); }
            set { SetValue(SourceGraphProperty, value); }
        }

        public int StartYear
        {
            get { return (int)GetValue(StartYearProperty); }
            set { SetValue(StartYearProperty, value); }
        }

        private static void ChangedMultiLocations(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as MainWindow;

            if (window.MultiLocations != null)
            {
                if (window.MultiLocations.Count > 0)
                {
                    // Calculate start year
                    window.StartYear = window.MultiLocations.Min(multiLocation => (int)multiLocation["StartYear"]);
                    window.SelectedYear = window.StartYear;
                }

                foreach (var multiLocation in window.MultiLocations)
                {
                    // Attach event so that we can load data when selection changes
                    // The -= ensures that events aren't subscribed twice
                    multiLocation.SelectionChanged -= window.multiLocation_SelectionChanged;
                    multiLocation.SelectionChanged += window.multiLocation_SelectionChanged;
                }
            }
        }

        private static void ChangedSelectedYear(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as MainWindow;
            window.UpdateGraphValue();
            window.UpdateMultiLocationValues();
        }

        private void multiLocation_SelectionChanged(object sender, ValueEventArgs<Location> e)
        {
            logger.Trace("");

            this.ReadGraphValueTimeSeries();
            this.UpdateGraphValue();
        }
    }
}