using Caching;
using LibBn;
using LibPipeline;
using MapControl;
using SharpKml.Dom;
using Smile;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using Telerik.Windows.Controls;

namespace KocViewer
{
    public partial class MainWindow : Window
    {
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

        public static readonly DependencyProperty IsTallySelectedProperty =
        DependencyProperty.Register("IsTallySelected", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty ProfileLocationsProperty =
        DependencyProperty.Register("ProfileLocations", typeof(IEnumerable<ILocation>), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedProfileLocationProperty =
        DependencyProperty.Register("SelectedProfileLocation", typeof(ILocation), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedTallyLocationProperty =
        DependencyProperty.Register("SelectedTallyLocation", typeof(ILocation), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedVertexValuesProperty =
        DependencyProperty.Register("SelectedVertexValues", typeof(IEnumerable<BnVertexValue>), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedYearProperty =
        DependencyProperty.Register("SelectedYear", typeof(int), typeof(MainWindow), new PropertyMetadata(Config.StartYear));

        public static readonly DependencyProperty TallyLocationsProperty =
        DependencyProperty.Register("TallyLocations", typeof(IEnumerable<ILocation>), typeof(MainWindow), new PropertyMetadata(null));

        public Dictionary<int, List<BnVertexValue>> DefaultVertexValuesByYear = new Dictionary<int, List<BnVertexValue>>();

        public BnInputStore InputManager = new BnInputStore();

        public SensorListener SensorListener = new SensorListener();

        public Dictionary<int, List<BnVertexValue>> VertexValuesByYear = new Dictionary<int, List<BnVertexValue>>();

        public MainWindow()
        {
            StyleManager.ApplicationTheme = new Windows8TouchTheme();
            InitializeComponent();

            this.SetBinding(MainWindow.IsSensorButtonVisibleProperty, new Binding
            {
                Path = new PropertyPath("IsSensorButtonVisible"),
                Source = Properties.Settings.Default
            });

            TileImageLoader.Cache = new ImageFileCache(TileImageLoader.DefaultCacheName, @"D:\Data\Cache");
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

                var vertexValues = bnUpdater.GetVertexValues(Config.NetworkFile, defaultInputs, userInputs, lastYearVertexValues);
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
    }
}