using LibBn;
using LibPipeline;
using SharpKml.Dom;
using Smile;
using System;
using System.Collections.Generic;
using System.Windows;
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

        public static readonly DependencyProperty PipelineProfileProperty =
        DependencyProperty.Register("PipelineProfile", typeof(Pipeline), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty PipelineTallyProperty =
        DependencyProperty.Register("PipelineTally", typeof(Pipeline), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedVertexValuesProperty =
        DependencyProperty.Register("SelectedVertexValues", typeof(IEnumerable<BnVertexValue>), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedYearProperty =
        DependencyProperty.Register("SelectedYear", typeof(int), typeof(MainWindow), new PropertyMetadata(Config.StartYear));

        public Dictionary<int, List<BnVertexValue>> DefaultVertexValuesByYear = new Dictionary<int, List<BnVertexValue>>();

        public BnInputStore InputManager = new BnInputStore();

        public InfoWindowViewModel PipelineProfileInfoWindowViewModel = new InfoWindowViewModel();

        public PipelineViewModel PipelineProfileViewModel = new PipelineViewModel();

        public InfoWindowViewModel PipelineTallyInfoWindowViewModel = new InfoWindowViewModel();

        public PipelineViewModel PipelineTallyViewModel = new PipelineViewModel();

        public SensorListener SensorListener = new SensorListener();

        public Dictionary<int, List<BnVertexValue>> VertexValuesByYear = new Dictionary<int, List<BnVertexValue>>();

        public MainWindow()
        {
            StyleManager.ApplicationTheme = new Windows8TouchTheme();
            InitializeComponent();
            InitializeDataContexts();
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

        public Pipeline PipelineProfile
        {
            get { return (Pipeline)GetValue(PipelineProfileProperty); }
            set { SetValue(PipelineProfileProperty, value); }
        }

        public Pipeline PipelineTally
        {
            get { return (Pipeline)GetValue(PipelineTallyProperty); }
            set { SetValue(PipelineTallyProperty, value); }
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

        public void AddInput(BnVertexViewModel vertexViewModel)
        {
            var vertexInput = this.InputManager.GetVertexInput(BnInputType.User, this.SelectedYear, vertexViewModel.Key);
            vertexInput.FillFrom(vertexViewModel);
        }

        public void InitializeDataContexts()
        {
            this.PipelineProfilePropertyGrid.DataContext = this.PipelineProfileViewModel;
            this.PipelineTallyPropertyGrid.DataContext = this.PipelineTallyViewModel;
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