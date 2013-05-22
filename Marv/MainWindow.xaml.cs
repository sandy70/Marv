using Caching;
using LibBn;
using LibPipeline;
using LibPipline;
using MapControl;
using SharpKml.Dom;
using Smile;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using Telerik.Windows.Controls;

namespace Marv
{
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty CacheDirectoryProperty =
        DependencyProperty.Register("CacheDirectory", typeof(string), typeof(MainWindow), new PropertyMetadata(".\\"));

        public static readonly DependencyProperty EndYearProperty =
        DependencyProperty.Register("EndYear", typeof(int), typeof(MainWindow), new PropertyMetadata(2010));

        public static readonly DependencyProperty FileNameProperty =
        DependencyProperty.Register("FileName", typeof(string), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty GroundOverlayProperty =
        DependencyProperty.Register("GroundOverlay", typeof(GroundOverlay), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty IsGroupButtonVisibleProperty =
        DependencyProperty.Register("IsGroupButtonVisible", typeof(bool), typeof(MainWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty IsProfileSelectedProperty =
        DependencyProperty.Register("IsProfileSelected", typeof(bool), typeof(MainWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty IsSensorButtonVisibleProperty =
        DependencyProperty.Register("IsSensorButtonVisible", typeof(bool), typeof(MainWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty IsSettingsVisibleProperty =
        DependencyProperty.Register("IsSettingsVisible", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty IsTallySelectedProperty =
        DependencyProperty.Register("IsTallySelected", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty ProfileLocationsProperty =
        DependencyProperty.Register("ProfileLocations", typeof(IEnumerable<ILocation>), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedProfileLocationProperty =
        DependencyProperty.Register("SelectedProfileLocation", typeof(ILocation), typeof(MainWindow), new PropertyMetadata(null, ChangedSelectedProfileLocation));

        public static readonly DependencyProperty SelectedTallyLocationProperty =
        DependencyProperty.Register("SelectedTallyLocation", typeof(ILocation), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedVertexValuesProperty =
        DependencyProperty.Register("SelectedVertexValues", typeof(IEnumerable<BnVertexValue>), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedYearProperty =
        DependencyProperty.Register("SelectedYear", typeof(int), typeof(MainWindow), new PropertyMetadata(2000, ChangedSelectedYear));

        public static readonly DependencyProperty SourceGraphProperty =
        DependencyProperty.Register("SourceGraph", typeof(BnGraph), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty StartingGroupProperty =
        DependencyProperty.Register("StartingGroup", typeof(string), typeof(MainWindow), new PropertyMetadata("all"));

        public static readonly DependencyProperty StartYearProperty =
        DependencyProperty.Register("StartYear", typeof(int), typeof(MainWindow), new PropertyMetadata(2000));

        public static readonly DependencyProperty TallyLocationsProperty =
        DependencyProperty.Register("TallyLocations", typeof(IEnumerable<ILocation>), typeof(MainWindow), new PropertyMetadata(null));

        public Dictionary<int, List<BnVertexValue>> DefaultVertexValuesByYear = new Dictionary<int, List<BnVertexValue>>();

        public BnInputStore InputManager = new BnInputStore();

        public SensorListener SensorListener = new SensorListener();

        public Dictionary<int, List<BnVertexValue>> VertexValuesByYear = new Dictionary<int, List<BnVertexValue>>();

        private ValueStore valueStore = new ValueStore();

        public MainWindow()
        {
            StyleManager.ApplicationTheme = new Windows8TouchTheme();
            InitializeComponent();

            TileImageLoader.Cache = new ImageFileCache(TileImageLoader.DefaultCacheName, this.CacheDirectory);
            this.MapView.TileLayer = TileLayers.MapBoxTerrain;
        }

        public string CacheDirectory
        {
            get { return (string)GetValue(CacheDirectoryProperty); }
            set { SetValue(CacheDirectoryProperty, value); }
        }

        public int EndYear
        {
            get { return (int)GetValue(EndYearProperty); }
            set { SetValue(EndYearProperty, value); }
        }

        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        public GroundOverlay GroundOverlay
        {
            get { return (GroundOverlay)GetValue(GroundOverlayProperty); }
            set { SetValue(GroundOverlayProperty, value); }
        }

        public bool IsGroupButtonVisible
        {
            get { return (bool)GetValue(IsGroupButtonVisibleProperty); }
            set { SetValue(IsGroupButtonVisibleProperty, value); }
        }

        public bool IsProfileSelected
        {
            get { return (bool)GetValue(IsProfileSelectedProperty); }
            set { SetValue(IsProfileSelectedProperty, value); }
        }

        public bool IsSensorButtonVisible
        {
            get { return (bool)GetValue(IsSensorButtonVisibleProperty); }
            set { SetValue(IsSensorButtonVisibleProperty, value); }
        }

        public bool IsSettingsVisible
        {
            get { return (bool)GetValue(IsSettingsVisibleProperty); }
            set { SetValue(IsSettingsVisibleProperty, value); }
        }

        public bool IsTallySelected
        {
            get { return (bool)GetValue(IsTallySelectedProperty); }
            set { SetValue(IsTallySelectedProperty, value); }
        }

        public IEnumerable<ILocation> ProfileLocations
        {
            get { return (IEnumerable<ILocation>)GetValue(ProfileLocationsProperty); }
            set { SetValue(ProfileLocationsProperty, value); }
        }

        public ILocation SelectedProfileLocation
        {
            get { return (ILocation)GetValue(SelectedProfileLocationProperty); }
            set { SetValue(SelectedProfileLocationProperty, value); }
        }

        public ILocation SelectedTallyLocation
        {
            get { return (ILocation)GetValue(SelectedTallyLocationProperty); }
            set { SetValue(SelectedTallyLocationProperty, value); }
        }

        public IEnumerable<BnVertexValue> SelectedVertexValues
        {
            get { return (IEnumerable<BnVertexValue>)GetValue(SelectedVertexValuesProperty); }
            set { SetValue(SelectedVertexValuesProperty, value); }
        }

        public int SelectedYear
        {
            get { return (int)GetValue(SelectedYearProperty); }
            set { SetValue(SelectedYearProperty, value); }
        }

        public BnGraph SourceGraph
        {
            get { return (BnGraph)GetValue(SourceGraphProperty); }
            set { SetValue(SourceGraphProperty, value); }
        }

        public string StartingGroup
        {
            get { return (string)GetValue(StartingGroupProperty); }
            set { SetValue(StartingGroupProperty, value); }
        }

        public int StartYear
        {
            get { return (int)GetValue(StartYearProperty); }
            set { SetValue(StartYearProperty, value); }
        }

        public IEnumerable<ILocation> TallyLocations
        {
            get { return (IEnumerable<ILocation>)GetValue(TallyLocationsProperty); }
            set { SetValue(TallyLocationsProperty, value); }
        }

        public void AddInput(BnVertexViewModel vertexViewModel)
        {
            var vertexInput = this.InputManager.GetVertexInput(BnInputType.User, this.SelectedYear, vertexViewModel.Key);
            vertexInput.FillFrom(vertexViewModel);
        }

        public void RemoveInput(BnVertexViewModel vertexViewModel)
        {
            this.InputManager.RemoveVertexInput(BnInputType.User, this.SelectedYear, vertexViewModel.Key);
        }

        public bool TryUpdateNetwork()
        {
            try
            {
                var defaultInputs = this.InputManager.GetGraphInput(BnInputType.Default, this.SelectedYear);
                var userInputs = this.InputManager.GetGraphInput(BnInputType.User, this.SelectedYear);

                var bnUpdater = new BnUpdater();

                List<BnVertexValue> lastYearVertexValues = null;

                if (this.SelectedYear == Config.StartYear)
                {
                    lastYearVertexValues = null;
                }
                else
                {
                    lastYearVertexValues = this.VertexValuesByYear[this.SelectedYear - 1];
                }

                var vertexValues = bnUpdater.GetVertexValues(this.FileName, defaultInputs, userInputs, lastYearVertexValues);
                this.VertexValuesByYear[this.SelectedYear] = vertexValues;
                this.GraphControl.SourceGraph.CopyFrom(vertexValues);
                this.GraphControl.SourceGraph.CalculateMostProbableStates();

                return true;
            }
            catch (SmileException exp)
            {
                return false;
            }
        }

        private static async void ChangedSelectedProfileLocation(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as MainWindow;

            if (!window.valueStore.HasLocationValue(window.SelectedProfileLocation))
            {
                window.PopupControl.ShowTextIndeterminate("Running model.");
                var locationValue = await MainWindow.RunModelAsync(window.SelectedProfileLocation, window.GraphControl.SourceGraph, window.StartYear, window.EndYear);
                window.GraphControl.SourceGraph.Value = locationValue[window.SelectedYear];
                window.valueStore.SetLocationValue(locationValue, window.SelectedProfileLocation);
                window.PopupControl.Hide();
            }
            else
            {
                var locationValue = window.valueStore.GetLocationValue(window.SelectedProfileLocation);
                window.GraphControl.SourceGraph.Value = locationValue[window.SelectedYear];
            }
        }

        private static async void ChangedSelectedYear(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as MainWindow;

            if (window.valueStore.HasGraphValue(window.SelectedYear, window.SelectedProfileLocation))
            {
                window.GraphControl.SourceGraph.Value = window.valueStore.GetGraphValue(window.SelectedYear, window.SelectedProfileLocation);
            }
            else
            {
                window.PopupControl.ShowTextIndeterminate("Running model.");
                var locationValue = await MainWindow.RunModelAsync(window.SelectedProfileLocation, window.GraphControl.SourceGraph, window.StartYear, window.EndYear);
                window.GraphControl.SourceGraph.Value = locationValue[window.SelectedYear];
                window.valueStore.SetLocationValue(locationValue, window.SelectedProfileLocation);
                window.PopupControl.Hide();
            }
        }

        private static Dictionary<int, Dictionary<string, Dictionary<string, double>>>
            RunModel(ILocation selectedLocation, BnGraph graph, int startYear, int endYear)
        {
            var inputStore = new InputStore();
            var locationValue = new Dictionary<int, Dictionary<string, Dictionary<string, double>>>();

            for (int year = startYear; year <= endYear; year++)
            {
                var graphInput = inputStore.GetGraphInput(year);
                var graphEvidence = new Dictionary<string, VertexEvidence>();

                var fixedVariables = new Dictionary<string, int>
                {
                    { "dia", 6 },
                    { "t", 5 },
                    { "coattype", 2 },
                    { "surfaceprep", 4 },
                    { "C8", 2 },
                    { "Kd", 0 },
                    { "Cs", 5 },
                    { "Rs", 4 },
                    { "pratio", 3 },
                    { "freq", 3 },
                    { "Kd_w", 10 },
                    { "Kd_b", 10 },
                    { "CP", 5 },
                    { "rho", 4 },
                    { "Co2", 3 },
                    { "millscale", 1 },
                    { "wd", 2 },
                    { "T", 5 },
                    { "P", 5 }
                };

                foreach (var variable in fixedVariables)
                {
                    graphEvidence[variable.Key] = new VertexEvidence
                    {
                        EvidenceType = EvidenceType.StateSelected,
                        StateIndex = variable.Value
                    };
                }

                var stateValues = new List<int> { 1000, 100, 95, 90, 85, 80, 75, 70, 65, 60, 55, 50, 45, 40, 35, 30, 25, 20, 15, 10, 5, 0 };

                var location = selectedLocation as dynamic;
                var mean = location.Pressure;
                var variance = Math.Pow(location.Pressure - location.Pressure_Min, 2);
                var normalDistribution = new NormalDistribution(mean, variance);

                graphEvidence["P"] = new VertexEvidence
                {
                    Evidence = new double[stateValues.Count - 1],
                    EvidenceType = EvidenceType.SoftEvidence
                };

                for (int i = 0; i < stateValues.Count - 1; i++)
                {
                    graphEvidence["P"].Evidence[i] = normalDistribution.CDF(stateValues[i]) - normalDistribution.CDF(stateValues[i + 1]);
                }

                graph.SetEvidence(graphEvidence);
                graph.UpdateBeliefs();

                locationValue[year] = graph.GetNetworkValue();
            }

            return locationValue;
        }

        private static Task<Dictionary<int, Dictionary<string, Dictionary<string, double>>>>
            RunModelAsync(ILocation selectedLocation, BnGraph graph, int startYear, int endYear)
        {
            return Task.Run(() => MainWindow.RunModel(selectedLocation, graph, startYear, endYear));
        }
    }
}