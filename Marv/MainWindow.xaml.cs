using Caching;
using LibBn;
using LibPipeline;
using MapControl;
using NDatabase;
using Smile;
using System;
using System.Collections.Generic;
using System.Windows;
using Telerik.Windows.Controls;
using System.Linq;
using NDatabase.Api;
using System.Threading.Tasks;

namespace Marv
{
    public partial class MainWindow : Window
    {
        public Dictionary<int, List<BnVertexValue>> DefaultVertexValuesByYear = new Dictionary<int, List<BnVertexValue>>();
        public BnInputStore InputManager = new BnInputStore();
        public PipelineValue PipelineValue = new PipelineValue();
        public SensorListener SensorListener = new SensorListener();
        public Dictionary<int, List<BnVertexValue>> VertexValuesByYear = new Dictionary<int, List<BnVertexValue>>();
        private NearNeutralPhSccModel model = new NearNeutralPhSccModel();
        private object _lock = new object();

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

                // var vertexValues = bnUpdater.GetVertexValues(this.FileName, defaultInputs, userInputs, lastYearVertexValues);
                // this.VertexValuesByYear[this.SelectedYear] = vertexValues;

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
            window.SelectedLocationValue = await window.LocationValueStore.GetLocationValueAsync(window.SelectedProfileLocation);
        }

        private static void ChangedSelectedYear(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as MainWindow;

            var modelValue = window.SelectedLocationValue[window.SelectedYear];

            foreach (var graph in window.Graphs)
            {
                var graphValue = modelValue[graph.Name];
                graph.Value = graphValue;
            }
        }
    }
}