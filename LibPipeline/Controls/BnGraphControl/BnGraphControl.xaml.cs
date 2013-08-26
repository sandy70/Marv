using LibNetwork;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LibPipeline
{
    public partial class BnGraphControl : UserControl
    {
        public static readonly DependencyProperty ConnectionColorProperty =
        DependencyProperty.Register("ConnectionColor", typeof(Color), typeof(BnGraphControl), new PropertyMetadata(Colors.LightSlateGray));

        public static readonly DependencyProperty DisplayGraphProperty =
        DependencyProperty.Register("DisplayGraph", typeof(BnGraph), typeof(BnGraphControl), new PropertyMetadata(null));

        public static readonly DependencyProperty GraphsProperty =
        DependencyProperty.Register("Graphs", typeof(IEnumerable<BnGraph>), typeof(BnGraphControl), new PropertyMetadata(null, ChangedGraphs));

        public static readonly DependencyProperty IncomingConnectionHighlightColorProperty =
        DependencyProperty.Register("IncomingConnectionHighlightColor", typeof(Color), typeof(BnGraphControl), new PropertyMetadata(Colors.SkyBlue));

        public static readonly DependencyProperty IsBackButtonVisibleProperty =
        DependencyProperty.Register("IsBackButtonVisible", typeof(bool), typeof(BnGraphControl), new PropertyMetadata(false));

        public static readonly DependencyProperty IsSensorButtonVisibleProperty =
        DependencyProperty.Register("IsSensorButtonVisible", typeof(bool), typeof(BnGraphControl), new PropertyMetadata(true));

        public static readonly RoutedEvent NewEvidenceAvailableEvent =
        EventManager.RegisterRoutedEvent("NewEvidenceAvailable", RoutingStrategy.Bubble, typeof(RoutedEventHandler<ValueEventArgs<BnVertexViewModel>>), typeof(BnGraphControl));

        public static readonly DependencyProperty OutgoingConnectionHighlightColorProperty =
        DependencyProperty.Register("OutgoingConnectionHighlightColor", typeof(Color), typeof(BnGraphControl), new PropertyMetadata(Colors.Red));

        public static readonly RoutedEvent RetractButtonClickedEvent =
        EventManager.RegisterRoutedEvent("RetractButtonClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler<ValueEventArgs<BnVertexViewModel>>), typeof(BnGraphControl));

        public static readonly DependencyProperty SelectedVertexViewModelProperty =
        DependencyProperty.Register("SelectedVertexViewModel", typeof(BnVertexViewModel), typeof(BnGraphControl), new PropertyMetadata(null));

        public static readonly RoutedEvent SensorButtonCheckedEvent =
        EventManager.RegisterRoutedEvent("SensorButtonChecked", RoutingStrategy.Bubble, typeof(RoutedEventHandler<ValueEventArgs<BnVertexViewModel>>), typeof(BnGraphControl));

        public static readonly RoutedEvent SensorButtonUncheckedEvent =
        EventManager.RegisterRoutedEvent("SensorButtonUnchecked", RoutingStrategy.Bubble, typeof(RoutedEventHandler<ValueEventArgs<BnVertexViewModel>>), typeof(BnGraphControl));

        public static readonly DependencyProperty ShapeOpacityProperty =
        DependencyProperty.Register("ShapeOpacity", typeof(double), typeof(BnGraphControl), new PropertyMetadata(1.0));

        public static readonly RoutedEvent StateDoubleClickedEvent =
        EventManager.RegisterRoutedEvent("StateDoubleClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler<BnGraphControlEventArgs>), typeof(BnGraphControl));

        public static readonly DependencyProperty ZoomProperty =
        DependencyProperty.Register("Zoom", typeof(double), typeof(BnGraphControl), new PropertyMetadata(1.0));

        private Dictionary<BnGraph, string> selectedGroups = new Dictionary<BnGraph, string>();

        public BnGraphControl()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler<ValueEventArgs<BnVertexViewModel>> NewEvidenceAvailable
        {
            add { AddHandler(NewEvidenceAvailableEvent, value); }
            remove { RemoveHandler(NewEvidenceAvailableEvent, value); }
        }

        public event RoutedEventHandler<ValueEventArgs<BnVertexViewModel>> RetractButtonClicked
        {
            add { AddHandler(RetractButtonClickedEvent, value); }
            remove { RemoveHandler(RetractButtonClickedEvent, value); }
        }

        public event RoutedEventHandler<ValueEventArgs<BnVertexViewModel>> SensorButtonChecked
        {
            add { AddHandler(SensorButtonCheckedEvent, value); }
            remove { RemoveHandler(SensorButtonCheckedEvent, value); }
        }

        public event RoutedEventHandler<ValueEventArgs<BnVertexViewModel>> SensorButtonUnchecked
        {
            add { AddHandler(SensorButtonUncheckedEvent, value); }
            remove { RemoveHandler(SensorButtonUncheckedEvent, value); }
        }

        public event RoutedEventHandler<BnGraphControlEventArgs> StateDoubleClicked
        {
            add { AddHandler(StateDoubleClickedEvent, value); }
            remove { RemoveHandler(StateDoubleClickedEvent, value); }
        }

        public Color ConnectionColor
        {
            get { return (Color)GetValue(ConnectionColorProperty); }
            set { SetValue(ConnectionColorProperty, value); }
        }

        public BnGraph DisplayGraph
        {
            get { return (BnGraph)GetValue(DisplayGraphProperty); }
            set { SetValue(DisplayGraphProperty, value); }
        }

        public IEnumerable<BnGraph> Graphs
        {
            get { return (IEnumerable<BnGraph>)GetValue(GraphsProperty); }
            set { SetValue(GraphsProperty, value); }
        }

        public Color IncomingConnectionHighlightColor
        {
            get { return (Color)GetValue(IncomingConnectionHighlightColorProperty); }
            set { SetValue(IncomingConnectionHighlightColorProperty, value); }
        }

        public bool IsBackButtonVisible
        {
            get { return (bool)GetValue(IsBackButtonVisibleProperty); }
            set { SetValue(IsBackButtonVisibleProperty, value); }
        }

        public bool IsSensorButtonVisible
        {
            get { return (bool)GetValue(IsSensorButtonVisibleProperty); }
            set { SetValue(IsSensorButtonVisibleProperty, value); }
        }

        public Color OutgoingConnectionHighlightColor
        {
            get { return (Color)GetValue(OutgoingConnectionHighlightColorProperty); }
            set { SetValue(OutgoingConnectionHighlightColorProperty, value); }
        }

        public Dictionary<BnGraph, string> SelectedGroups
        {
            get
            {
                return selectedGroups;
            }

            set
            {
                selectedGroups = value;
            }
        }

        public BnVertexViewModel SelectedVertexViewModel
        {
            get { return (BnVertexViewModel)GetValue(SelectedVertexViewModelProperty); }
            set { SetValue(SelectedVertexViewModelProperty, value); }
        }

        public double ShapeOpacity
        {
            get { return (double)GetValue(ShapeOpacityProperty); }
            set { SetValue(ShapeOpacityProperty, value); }
        }

        public double Zoom
        {
            get { return (double)GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty, value); }
        }

        public void UpdateDisplayGraphToDefaultGroups()
        {
            var displayGraph = new BnGraph();

            if (this.Graphs != null && this.Graphs.Count() > 0)
            {
                foreach (var graph in this.Graphs)
                {
                    this.selectedGroups[graph] = graph.DefaultGroup;
                    displayGraph.Add(graph.GetGroup(graph.DefaultGroup));
                }
            }

            this.DisplayGraph = displayGraph;
        }

        private static void ChangedGraphs(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var graphControl = d as BnGraphControl;

            if (graphControl.Graphs != null)
            {
                if (graphControl.Graphs is INotifyCollectionChanged)
                {
                    (graphControl.Graphs as INotifyCollectionChanged).CollectionChanged += (o1, e1) =>
                        {
                            graphControl.UpdateDisplayGraphToDefaultGroups();
                            graphControl.AttachHandler();
                        };
                }

                graphControl.UpdateDisplayGraphToDefaultGroups();
                graphControl.AttachHandler();
            }
        }

        private void AttachHandler()
        {
            foreach (var graph in this.Graphs)
            {
                foreach (var vertex in graph.Vertices)
                {
                    vertex.PropertyChanged += (o2, e2) =>
                    {
                        if (e2.PropertyName.Equals("DisplayPosition"))
                        {
                            vertex.Positions[this.selectedGroups[graph]] = vertex.DisplayPosition;
                        }
                    };
                }
            }
        }
    }
}