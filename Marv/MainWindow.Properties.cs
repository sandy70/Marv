using System;
using System.Collections.ObjectModel;
using System.Windows;
using Marv;
using Marv.Map;
using Marv.Controls;

namespace Marv
{
    public partial class MainWindow
    {
        public static readonly DependencyProperty CacheDirectoryProperty =
        DependencyProperty.Register("CacheDirectory", typeof(string), typeof(MainWindow), new PropertyMetadata(".\\"));

        public static readonly DependencyProperty ChartSeriesProperty =
        DependencyProperty.Register("ChartSeries", typeof(ModelCollection<IChartSeries>), typeof(MainWindow), new PropertyMetadata(new ModelCollection<IChartSeries>()));

        public static readonly DependencyProperty EarthquakesProperty =
        DependencyProperty.Register("Earthquakes", typeof(ModelCollection<Location>), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty EndYearProperty =
        DependencyProperty.Register("EndYear", typeof(int), typeof(MainWindow), new PropertyMetadata(2010));

        public static readonly DependencyProperty InputDirProperty =
        DependencyProperty.Register("InputDir", typeof(string), typeof(MainWindow), new PropertyMetadata(""));

        public static readonly DependencyProperty InputFileNameProperty =
        DependencyProperty.Register("InputFileName", typeof(string), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty IsBackButtonVisibleProperty =
        DependencyProperty.Register("IsBackButtonVisible", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty IsChartControlVisibleProperty =
        DependencyProperty.Register("IsChartControlVisible", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty IsLogoVisibleProperty =
        DependencyProperty.Register("IsLogoVisible", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty IsMapVisibleProperty =
        DependencyProperty.Register("IsMapVisible", typeof(bool), typeof(MainWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty IsMenuVisibleProperty =
        DependencyProperty.Register("IsMenuVisible", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty IsPropertyGridVisibleProperty =
        DependencyProperty.Register("IsPropertyGridVisible", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty IsSettingsControlVisibleProperty =
        DependencyProperty.Register("IsSettingsControlVisible", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty IsSettingsVisibleProperty =
        DependencyProperty.Register("IsSettingsVisible", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty IsTallySelectedProperty =
        DependencyProperty.Register("IsTallySelected", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty IsYearSliderVisibleProperty =
        DependencyProperty.Register("IsYearSliderVisible", typeof(bool), typeof(MainWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty MultiLocationValueTimeSeriesProperty =
        DependencyProperty.Register("MultiLocationValueTimeSeries", typeof(Dictionary<int, string, double>), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty NetworkFileNameProperty =
        DependencyProperty.Register("NetworkFileName", typeof(string), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty NotificationsProperty =
        DependencyProperty.Register("Notifications", typeof(ObservableCollection<INotification>), typeof(MainWindow), new PropertyMetadata(new ObservableCollection<INotification>()));

        public static readonly DependencyProperty PolylinesProperty =
        DependencyProperty.Register("Polylines", typeof(ModelCollection<LocationCollection>), typeof(MainWindow), new PropertyMetadata(null, OnChangedPolylines));

        public static readonly DependencyProperty RiskValueToBrushMapProperty =
        DependencyProperty.Register("RiskValueToBrushMap", typeof(RiskValueToBrushMap), typeof(MainWindow), new PropertyMetadata(new RiskValueToBrushMap()));

        public static readonly DependencyProperty SelectedYearProperty =
        DependencyProperty.Register("SelectedYear", typeof(int), typeof(MainWindow), new PropertyMetadata(2001, OnSelectedYearChanged));

        public static readonly DependencyProperty GraphProperty =
        DependencyProperty.Register("Graph", typeof(Graph), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty StartYearProperty =
        DependencyProperty.Register("StartYear", typeof(int), typeof(MainWindow), new PropertyMetadata(2001));

        public static readonly DependencyProperty SynergiModelProperty =
        DependencyProperty.Register("SynergiModel", typeof(SynergiModel), typeof(MainWindow), new PropertyMetadata(new SynergiModel()));

        public event EventHandler<ModelCollection<LocationCollection>> PolylinesChanged;

        public event EventHandler<double> SelectedYearChanged;

        public string CacheDirectory
        {
            get { return (string)GetValue(CacheDirectoryProperty); }
            set { SetValue(CacheDirectoryProperty, value); }
        }

        public ModelCollection<IChartSeries> ChartSeries
        {
            get { return (ModelCollection<IChartSeries>)GetValue(ChartSeriesProperty); }
            set { SetValue(ChartSeriesProperty, value); }
        }

        public ModelCollection<Location> Earthquakes
        {
            get { return (ModelCollection<Location>)GetValue(EarthquakesProperty); }
            set { SetValue(EarthquakesProperty, value); }
        }

        public int EndYear
        {
            get { return (int)GetValue(EndYearProperty); }
            set { SetValue(EndYearProperty, value); }
        }

        public string InputDir
        {
            get { return (string)GetValue(InputDirProperty); }
            set { SetValue(InputDirProperty, value); }
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

        public bool IsChartControlVisible
        {
            get { return (bool)GetValue(IsChartControlVisibleProperty); }
            set { SetValue(IsChartControlVisibleProperty, value); }
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

        public Dictionary<int, string, double> MultiLocationValueTimeSeries
        {
            get { return (Dictionary<int, string, double>)GetValue(MultiLocationValueTimeSeriesProperty); }
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

        public ModelCollection<LocationCollection> Polylines
        {
            get { return (ModelCollection<LocationCollection>)GetValue(PolylinesProperty); }
            set { SetValue(PolylinesProperty, value); }
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

        public Graph Graph
        {
            get { return (Graph)GetValue(GraphProperty); }
            set { SetValue(GraphProperty, value); }
        }

        public int StartYear
        {
            get { return (int)GetValue(StartYearProperty); }
            set { SetValue(StartYearProperty, value); }
        }

        public SynergiModel SynergiModel
        {
            get { return (SynergiModel)GetValue(SynergiModelProperty); }
            set { SetValue(SynergiModelProperty, value); }
        }

        private static void OnChangedPolylines(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as MainWindow;

            if (window.PolylinesChanged != null)
            {
                window.PolylinesChanged(window, window.Polylines);
            }
        }

        private static void OnSelectedYearChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as MainWindow;

            if (window.SelectedYearChanged != null)
            {
                window.SelectedYearChanged(window, window.SelectedYear);
            }
        }
    }
}