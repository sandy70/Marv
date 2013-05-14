using LibBn;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Telerik.Windows.Controls;

namespace LibPipeline
{
    public partial class BnGraphControl : UserControl
    {
        public static readonly DependencyProperty ConnectionColorProperty =
        DependencyProperty.Register("ConnectionColor", typeof(Color), typeof(BnGraphControl), new PropertyMetadata(Colors.LightSlateGray));

        public static readonly DependencyProperty DisplayGraphProperty =
        DependencyProperty.Register("DisplayGraph", typeof(BnGraph), typeof(BnGraphControl), new PropertyMetadata(null));

        public static readonly DependencyProperty FileNameProperty =
        DependencyProperty.Register("FileName", typeof(string), typeof(BnGraphControl), new PropertyMetadata(null, ChangedFileName));

        public static readonly RoutedEvent FileNotFoundEvent =
        EventManager.RegisterRoutedEvent("FileNotFound", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BnGraphControl));

        public static readonly RoutedEvent GroupButtonClickedEvent =
        EventManager.RegisterRoutedEvent("GroupButtonClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler<ValueEventArgs<BnVertexViewModel>>), typeof(BnGraphControl));

        public static readonly DependencyProperty IncomingConnectionHighlightColorProperty =
        DependencyProperty.Register("IncomingConnectionHighlightColor", typeof(Color), typeof(BnGraphControl), new PropertyMetadata(Colors.SkyBlue));

        public static readonly DependencyProperty IsGroupButtonVisibleProperty =
        DependencyProperty.Register("IsGroupButtonVisible", typeof(bool), typeof(BnGraphControl), new PropertyMetadata(true));

        public static readonly DependencyProperty IsGroupedProperty =
        DependencyProperty.Register("IsGrouped", typeof(bool), typeof(BnGraphControl), new PropertyMetadata(true, ChangedIsGrouped));

        public static readonly DependencyProperty IsSensorButtonVisibleProperty =
        DependencyProperty.Register("IsSensorButtonVisible", typeof(bool), typeof(BnGraphControl), new PropertyMetadata(true));

        public static readonly RoutedEvent NewEvidenceAvailableEvent =
        EventManager.RegisterRoutedEvent("NewEvidenceAvailable", RoutingStrategy.Bubble, typeof(RoutedEventHandler<ValueEventArgs<BnVertexViewModel>>), typeof(BnGraphControl));

        public static readonly DependencyProperty OutgoingConnectionHighlightColorProperty =
        DependencyProperty.Register("OutgoingConnectionHighlightColor", typeof(Color), typeof(BnGraphControl), new PropertyMetadata(Colors.Red));

        public static readonly RoutedEvent RetractButtonClickedEvent =
        EventManager.RegisterRoutedEvent("RetractButtonClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler<ValueEventArgs<BnVertexViewModel>>), typeof(BnGraphControl));

        public static readonly DependencyProperty SelectedGroupProperty =
        DependencyProperty.Register("SelectedGroup", typeof(string), typeof(BnGraphControl), new PropertyMetadata(null, ChangedSelectedGroup));

        public static readonly DependencyProperty SelectedVertexViewModelProperty =
        DependencyProperty.Register("SelectedVertexViewModel", typeof(BnVertexViewModel), typeof(BnGraphControl), new PropertyMetadata(null));

        public static readonly RoutedEvent SensorButtonCheckedEvent =
        EventManager.RegisterRoutedEvent("SensorButtonChecked", RoutingStrategy.Bubble, typeof(RoutedEventHandler<ValueEventArgs<BnVertexViewModel>>), typeof(BnGraphControl));

        public static readonly RoutedEvent SensorButtonUncheckedEvent =
        EventManager.RegisterRoutedEvent("SensorButtonUnchecked", RoutingStrategy.Bubble, typeof(RoutedEventHandler<ValueEventArgs<BnVertexViewModel>>), typeof(BnGraphControl));

        public static readonly DependencyProperty ShapeOpacityProperty =
        DependencyProperty.Register("ShapeOpacity", typeof(double), typeof(BnGraphControl), new PropertyMetadata(1.0));

        public static readonly DependencyProperty SourceGraphProperty =
        DependencyProperty.Register("SourceGraph", typeof(BnGraph), typeof(BnGraphControl), new PropertyMetadata(null, ChangedSourceGraph));

        public static readonly DependencyProperty StartingGroupProperty =
        DependencyProperty.Register("StartingGroup", typeof(string), typeof(BnGraphControl), new PropertyMetadata("all"));

        public static readonly DependencyProperty VertexValuesProperty =
        DependencyProperty.Register("VertexValues", typeof(IEnumerable<BnVertexValue>), typeof(BnGraphControl), new PropertyMetadata(null, ChangedVertexValues));

        public BnGraphControl()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler FileNotFound
        {
            add { AddHandler(FileNotFoundEvent, value); }
            remove { RemoveHandler(FileNotFoundEvent, value); }
        }

        public event RoutedEventHandler<ValueEventArgs<BnVertexViewModel>> GroupButtonClicked
        {
            add { AddHandler(GroupButtonClickedEvent, value); }
            remove { RemoveHandler(GroupButtonClickedEvent, value); }
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

        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        public Color IncomingConnectionHighlightColor
        {
            get { return (Color)GetValue(IncomingConnectionHighlightColorProperty); }
            set { SetValue(IncomingConnectionHighlightColorProperty, value); }
        }

        public bool IsGroupButtonVisible
        {
            get { return (bool)GetValue(IsGroupButtonVisibleProperty); }
            set { SetValue(IsGroupButtonVisibleProperty, value); }
        }

        public bool IsGrouped
        {
            get { return (bool)GetValue(IsGroupedProperty); }
            set { SetValue(IsGroupedProperty, value); }
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

        public string SelectedGroup
        {
            get { return (string)GetValue(SelectedGroupProperty); }
            set { SetValue(SelectedGroupProperty, value); }
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

        public IEnumerable<BnVertexValue> VertexValues
        {
            get { return (IEnumerable<BnVertexValue>)GetValue(VertexValuesProperty); }
            set { SetValue(VertexValuesProperty, value); }
        }

        private async static void ChangedFileName(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var graphControl = d as BnGraphControl;
            var fileName = e.NewValue as string;

            if (File.Exists(graphControl.FileName))
            {
                graphControl.SourceGraph = await BnGraphReader<BnVertexViewModel>.ReadAsync(graphControl.FileName);

                // We need to do this so that ChangedSelectedGroup is fired
                graphControl.SelectedGroup = null;
                graphControl.SelectedGroup = graphControl.StartingGroup;
            }
            else
            {
                graphControl.RaiseEvent(new ValueEventArgs<string>
                {
                    RoutedEvent = BnGraphControl.FileNotFoundEvent,
                });
            }
        }

        private static void ChangedIsGrouped(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var graphControl = d as BnGraphControl;

            if (!graphControl.IsGrouped)
            {
                PartGraphGenerator partGraphGenerator = new PartGraphGenerator();
                graphControl.DisplayGraph = partGraphGenerator.Generate(graphControl.SourceGraph, Groups.All);
                graphControl.HighlightGroup(graphControl.SelectedGroup);
            }
            else
            {
                PartGraphGenerator partGraphGenerator = new PartGraphGenerator();
                graphControl.DisplayGraph = partGraphGenerator.Generate(graphControl.SourceGraph, graphControl.SelectedGroup);
            }
        }

        private static void ChangedSelectedGroup(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var graphControl = d as BnGraphControl;

            if (graphControl.IsGrouped)
            {
                graphControl.HighlightGroup(graphControl.SelectedGroup);
                BnGraphControl.UpdateDisplayGraph(graphControl);
            }
            else
            {
                graphControl.HighlightGroup(graphControl.SelectedGroup);
            }
        }

        private static void ChangedSourceGraph(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var graphControl = d as BnGraphControl;

            if (graphControl != null && graphControl.SourceGraph != null)
            {
                foreach (var vertex in graphControl.SourceGraph.Vertices)
                {
                    BnVertexViewModel vertexViewModel = vertex as BnVertexViewModel;

                    vertexViewModel.PropertyChanged += (o, a) =>
                    {
                        if (a.PropertyName.Equals("DisplayPosition"))
                        {
                            if (graphControl.IsGrouped)
                            {
                                vertexViewModel.PositionsByGroup[graphControl.SelectedGroup] = vertexViewModel.DisplayPosition;
                            }
                            else
                            {
                                vertexViewModel.PositionsByGroup[Groups.All] = vertexViewModel.DisplayPosition;
                            }
                        }
                    };
                }

                graphControl.SourceGraph.CalculateMostProbableStates();
            }
        }

        private static void ChangedVertexValues(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var graphControl = d as BnGraphControl;

            if (graphControl.SourceGraph != null)
            {
                graphControl.SourceGraph.CopyFrom(graphControl.VertexValues);
                graphControl.SourceGraph.CalculateMostProbableStates();
            }
        }

        private static void UpdateDisplayGraph(BnGraphControl graphControl)
        {
            if (graphControl.SelectedGroup == null || graphControl.SourceGraph == null)
            {
                return;
            }
            else
            {
                PartGraphGenerator partGraphGenerator = new PartGraphGenerator();
                graphControl.DisplayGraph = partGraphGenerator.Generate(graphControl.SourceGraph, graphControl.SelectedGroup);

                if (graphControl.SelectedGroup.Equals(Groups.Default))
                {
                    graphControl.IsGroupButtonVisible = true;
                    graphControl.BackButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    graphControl.IsGroupButtonVisible = false;
                    graphControl.BackButton.Visibility = Visibility.Visible;
                }
            }
        }

        private void HighlightGroup(string group)
        {
            var fadeOutAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0.2,
                BeginTime = TimeSpan.FromSeconds(0.5),
                Duration = new Duration(TimeSpan.FromMilliseconds(300))
            };

            var fadeInAnimation = new DoubleAnimation
            {
                From = 0.2,
                To = 1,
                BeginTime = TimeSpan.FromSeconds(0.5),
                Duration = new Duration(TimeSpan.FromMilliseconds(300))
            };

            foreach (var shape in this.DiagramPart.Shapes)
            {
                var vertexViewModel = (shape as RadDiagramShape).DataContext as BnVertexViewModel;

                if (vertexViewModel.Groups.Contains(group))
                {
                    (shape as RadDiagramShape).BeginAnimation(RadDiagramShape.OpacityProperty, fadeInAnimation);
                }
                else
                {
                    (shape as RadDiagramShape).BeginAnimation(RadDiagramShape.OpacityProperty, fadeOutAnimation);
                }
            }
        }
    }
}