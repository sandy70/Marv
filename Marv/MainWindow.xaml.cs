using Caching;
using LibBn;
using LibPipeline;
using MapControl;
using Smile;
using System.Collections.Generic;
using System.Windows;
using Telerik.Windows.Controls;

namespace Marv
{
    public partial class MainWindow : Window
    {
        public Dictionary<int, List<BnVertexValue>> DefaultVertexValuesByYear = new Dictionary<int, List<BnVertexValue>>();

        public BnInputStore InputManager = new BnInputStore();

        public SensorListener SensorListener = new SensorListener();

        public Dictionary<int, List<BnVertexValue>> VertexValuesByYear = new Dictionary<int, List<BnVertexValue>>();

        private Model model = new Model();

        private ValueStore valueStore = new ValueStore();

        public MainWindow()
        {
            StyleManager.ApplicationTheme = new Windows8TouchTheme();
            InitializeComponent();

            TileImageLoader.Cache = new ImageFileCache(TileImageLoader.DefaultCacheName, this.CacheDirectory);
            this.MapView.TileLayer = TileLayers.MapBoxTerrain;
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

                // this.GraphControl.SourceGraph.CopyFrom(vertexValues);
                // this.GraphControl.SourceGraph.UpdateMostProbableStates();

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

            if (window.valueStore.HasGraphValues(window.SelectedYear, window.SelectedProfileLocation, window.Graphs))
            {
                foreach (var graph in window.Graphs)
                {
                    graph.Value = window.valueStore.GetGraphValue(window.SelectedYear, window.SelectedProfileLocation, graph);
                }
            }
            else
            {
                window.PopupControl.ShowTextIndeterminate("Running model.");

                window.Model.SelectedLocation = window.SelectedProfileLocation;
                var intervalValues = await window.Model.RunAsync();

                foreach (var graph in intervalValues.Keys)
                {
                    graph.Value = intervalValues[graph][window.SelectedYear];
                    window.valueStore.SetIntervalValue(intervalValues[graph], window.SelectedProfileLocation, graph);
                }

                window.PopupControl.Hide();
            }
        }

        private static async void ChangedSelectedYear(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as MainWindow;

            if (window.valueStore.HasGraphValues(window.SelectedYear, window.SelectedProfileLocation, window.Graphs))
            {
                foreach (var graph in window.Graphs)
                {
                    graph.Value = window.valueStore.GetGraphValue(window.SelectedYear, window.SelectedProfileLocation, graph);
                }
            }
            else
            {
                window.PopupControl.ShowTextIndeterminate("Running model.");

                window.Model.SelectedLocation = window.SelectedProfileLocation;
                var intervalValues = await window.Model.RunAsync();

                foreach (var graph in intervalValues.Keys)
                {
                    graph.Value = intervalValues[graph][window.SelectedYear];
                    window.valueStore.SetIntervalValue(intervalValues[graph], window.SelectedProfileLocation, graph);
                }

                window.PopupControl.Hide();
            }
        }
    }
}