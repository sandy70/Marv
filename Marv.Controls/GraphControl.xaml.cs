using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Marv.Common;
using Telerik.Windows.Controls;
using Telerik.Windows.Diagrams.Core;
using Orientation = Telerik.Windows.Diagrams.Core.Orientation;

namespace Marv.Controls
{
    public partial class GraphControl : INotifyPropertyChanged, INotifier
    {
        public static readonly DependencyProperty AutoSaveDurationProperty =
            DependencyProperty.Register("AutoSaveDuration", typeof (int), typeof (GraphControl), new PropertyMetadata(10000));

        public static readonly DependencyProperty ConnectionColorProperty =
            DependencyProperty.Register("ConnectionColor", typeof (Color), typeof (GraphControl), new PropertyMetadata(Colors.LightSlateGray));

        public static readonly DependencyProperty GraphProperty =
            DependencyProperty.Register("Graph", typeof (Graph), typeof (GraphControl), new PropertyMetadata(null, ChangedGraph));

        public static readonly DependencyProperty IncomingConnectionHighlightColorProperty =
            DependencyProperty.Register("IncomingConnectionHighlightColor", typeof (Color), typeof (GraphControl), new PropertyMetadata(Colors.SkyBlue));

        public static readonly DependencyProperty IsAdvancedToolbarVisibleProperty =
            DependencyProperty.Register("IsAdvancedToolbarVisible", typeof (bool), typeof (GraphControl), new PropertyMetadata(true));

        public static readonly DependencyProperty IsAutoLayoutEnabledProperty =
            DependencyProperty.Register("IsAutoLayoutEnabled", typeof (bool), typeof (GraphControl), new PropertyMetadata(false));

        public static readonly DependencyProperty IsAutoSaveEnabledProperty =
            DependencyProperty.Register("IsAutoSaveEnabled", typeof (bool), typeof (GraphControl), new PropertyMetadata(true));

        public static readonly DependencyProperty IsInputVisibleProperty =
            DependencyProperty.Register("IsInputVisible", typeof (bool), typeof (GraphControl), new PropertyMetadata(false));

        public static readonly DependencyProperty IsNavigationPaneVisibleProperty =
            DependencyProperty.Register("IsNavigationPaneVisible", typeof (bool), typeof (GraphControl), new PropertyMetadata(false));

        public static readonly DependencyProperty IsVerticesEnabledProperty =
            DependencyProperty.Register("IsVerticesEnabled", typeof (bool), typeof (GraphControl), new PropertyMetadata(true));

        public static readonly DependencyProperty NetworkProperty =
            DependencyProperty.Register("Network", typeof (Network), typeof (GraphControl), new PropertyMetadata(null));

        public static readonly DependencyProperty OutgoingConnectionHighlightColorProperty =
            DependencyProperty.Register("OutgoingConnectionHighlightColor", typeof (Color), typeof (GraphControl), new PropertyMetadata(Colors.Red));

        public static readonly DependencyProperty ShapeOpacityProperty =
            DependencyProperty.Register("ShapeOpacity", typeof (double), typeof (GraphControl), new PropertyMetadata(1.0));

        private Graph displayGraph;
        private string displayVertexKey;
        private bool isConnectorsManipulationEnabled;
        private bool isDefaultGroupVisible;
        private bool isManipulationAdornerVisible;
        private string selectedGroup;

        public int AutoSaveDuration
        {
            get { return (int) this.GetValue(AutoSaveDurationProperty); }

            set { this.SetValue(AutoSaveDurationProperty, value); }
        }

        public Color ConnectionColor
        {
            get { return (Color) this.GetValue(ConnectionColorProperty); }

            set { this.SetValue(ConnectionColorProperty, value); }
        }

        public Graph DisplayGraph
        {
            get { return this.displayGraph; }

            set
            {
                if (value.Equals(this.displayGraph))
                {
                    return;
                }

                this.displayGraph = value;
                this.RaisePropertyChanged();
            }
        }

        public Graph Graph
        {
            get { return (Graph) this.GetValue(GraphProperty); }

            set { this.SetValue(GraphProperty, value); }
        }

        public Color IncomingConnectionHighlightColor
        {
            get { return (Color) this.GetValue(IncomingConnectionHighlightColorProperty); }

            set { this.SetValue(IncomingConnectionHighlightColorProperty, value); }
        }

        public bool IsAdvancedToolbarVisible
        {
            get { return (bool) this.GetValue(IsAdvancedToolbarVisibleProperty); }
            set { this.SetValue(IsAdvancedToolbarVisibleProperty, value); }
        }

        public bool IsAutoLayoutEnabled
        {
            get { return (bool) this.GetValue(IsAutoLayoutEnabledProperty); }
            set { this.SetValue(IsAutoLayoutEnabledProperty, value); }
        }

        public bool IsAutoSaveEnabled
        {
            get { return (bool) this.GetValue(IsAutoSaveEnabledProperty); }

            set { this.SetValue(IsAutoSaveEnabledProperty, value); }
        }

        public bool IsConnectorsManipulationEnabled
        {
            get { return this.isConnectorsManipulationEnabled; }

            set
            {
                if (value.Equals(this.isConnectorsManipulationEnabled))
                {
                    return;
                }

                this.isConnectorsManipulationEnabled = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsDefaultGroupVisible
        {
            get { return this.isDefaultGroupVisible; }

            set
            {
                if (value.Equals(this.isDefaultGroupVisible))
                {
                    return;
                }

                this.isDefaultGroupVisible = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsInputVisible
        {
            get { return (bool) this.GetValue(IsInputVisibleProperty); }

            set { this.SetValue(IsInputVisibleProperty, value); }
        }

        public bool IsManipulationAdornerVisible
        {
            get { return this.isManipulationAdornerVisible; }

            set
            {
                if (value.Equals(this.isManipulationAdornerVisible))
                {
                    return;
                }

                this.isManipulationAdornerVisible = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsNavigationPaneVisible
        {
            get { return (bool) this.GetValue(IsNavigationPaneVisibleProperty); }
            set { this.SetValue(IsNavigationPaneVisibleProperty, value); }
        }

        public bool IsVerticesEnabled
        {
            get { return (bool) this.GetValue(IsVerticesEnabledProperty); }

            set { this.SetValue(IsVerticesEnabledProperty, value); }
        }

        public Network Network
        {
            get { return (Network) GetValue(NetworkProperty); }
            set { SetValue(NetworkProperty, value); }
        }

        public Color OutgoingConnectionHighlightColor
        {
            get { return (Color) this.GetValue(OutgoingConnectionHighlightColorProperty); }

            set { this.SetValue(OutgoingConnectionHighlightColorProperty, value); }
        }

        public string SelectedGroup
        {
            get { return this.selectedGroup; }

            set
            {
                if (value.Equals(this.selectedGroup))
                {
                    return;
                }

                this.selectedGroup = value;
                this.RaisePropertyChanged();
            }
        }

        public double ShapeOpacity
        {
            get { return (double) this.GetValue(ShapeOpacityProperty); }

            set { this.SetValue(ShapeOpacityProperty, value); }
        }

        public GraphControl()
        {
            InitializeComponent();
            this.InitializeAutoSave();
        }

        private static void ChangedGraph(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as GraphControl;

            if (control == null || control.Graph == null)
            {
                return;
            }

            var oldGraph = e.OldValue as Graph;

            control.RaiseGraphChanged(e.NewValue as Graph, oldGraph);

            if (control.Graph.Vertices.Count > 0)
            {
                control.Graph.SelectedVertex = control.Graph.GetSink();
            }

            control.SelectedGroup = control.Graph.DefaultGroup;
        }

        public void Open(string fileName)
        {
            this.Network = Network.Read(fileName);

            this.Graph = Graph.Read(this.Network);
            this.Graph.Belief = this.Network.GetBeliefs();

            var selectedVertex = this.Graph.SelectedVertex ?? this.Graph.Vertices.FirstOrDefault(vertex => vertex.HeaderOfGroup == this.SelectedGroup);
            this.UpdateDisplayGraph(this.SelectedGroup, selectedVertex == null ? null : selectedVertex.Key);
        }

        public void RaiseNotificationClosed(Notification notification)
        {
            if (this.NotificationClosed != null)
            {
                this.NotificationClosed(this, notification);
            }
        }

        private void AutoFitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Diagram.AutoFit(new Thickness(10));
        }

        private void AutoLayoutButton_Checked(object sender, RoutedEventArgs e)
        {
            this.DisableVertexDragging();
            this.UpdateLayout();
        }

        private void AutoLayoutButton_Unchecked(object sender, RoutedEventArgs e)
        {
            this.EnableVertexDragging();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.SelectedGroup = this.Graph.DefaultGroup;
        }

        private void BringShapeToFront(IShape shape)
        {
            // Bring shape to front
            this.Diagram.BringToFront(new List<IDiagramItem>
            {
                shape
            });

            // Change color of connections
            foreach (var conn in this.Diagram.Connections)
            {
                (conn as RadDiagramConnection).Stroke = new SolidColorBrush(this.ConnectionColor);
            }

            foreach (var conn in shape.IncomingLinks)
            {
                (conn as RadDiagramConnection).Stroke = new SolidColorBrush(this.IncomingConnectionHighlightColor);
            }

            foreach (var conn in shape.OutgoingLinks)
            {
                (conn as RadDiagramConnection).Stroke = new SolidColorBrush(this.OutgoingConnectionHighlightColor);
            }

            // Pan shape into view if required
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(150)
            };

            timer.Tick += (o, args) =>
            {
                if (!shape.Bounds.IsInBounds(this.Diagram.Viewport))
                {
                    var offset = this.Diagram.Viewport.GetOffset(shape.Bounds, 20);

                    // Extension OffsetRect is part of Telerik.Windows.Diagrams.Core
                    this.Diagram.BringIntoView(this.Diagram.Viewport.OffsetRect(offset.X, offset.Y));
                }

                timer.Stop();
            };

            timer.Start();
        }

        private void ClearEvidenceButton_Click(object sender, RoutedEventArgs e)
        {
            this.Graph.Evidence = null;
            this.RaiseEvidenceEntered();
        }

        private void ConnectorButton_Checked(object sender, RoutedEventArgs e)
        {
            this.EnableConnectorEditing();
        }

        private void ConnectorButton_Unchecked(object sender, RoutedEventArgs e)
        {
            this.DisableConnectorEditing();
        }

        private void DisableConnectorEditing()
        {
            this.IsConnectorsManipulationEnabled = false;
            this.IsManipulationAdornerVisible = false;
            this.IsVerticesEnabled = true;
        }

        private void DisableVertexDragging()
        {
            if (this.Graph == null)
            {
                return;
            }

            foreach (var vertex in this.Graph.Vertices)
            {
                vertex.IsDraggingEnabled = false;
            }
        }

        private void EnableConnectorEditing()
        {
            this.IsConnectorsManipulationEnabled = true;
            this.IsManipulationAdornerVisible = true;
            this.IsVerticesEnabled = false;
        }

        private void EnableVertexDragging()
        {
            foreach (var vertex in this.Graph.Vertices)
            {
                vertex.IsDraggingEnabled = true;
            }
        }

        private void ExpandButton_Click(object sender, RoutedEventArgs e)
        {
            this.DisplayGraph.IsExpanded = !this.DisplayGraph.IsMostlyExpanded;
            this.UpdateLayout();
        }

        private void GraphControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.PropertyChanged -= GraphControl_PropertyChanged;
            this.PropertyChanged += GraphControl_PropertyChanged;
        }

        private void GraphControl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedGroup")
            {
                var selectedVertex = this.Graph.SelectedVertex ?? this.Graph.Vertices.FirstOrDefault(vertex => vertex.HeaderOfGroup == this.SelectedGroup);
                this.UpdateDisplayGraph(this.SelectedGroup, selectedVertex == null ? null : selectedVertex.Key);
            }
        }

        private void GraphControl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Graph.Write(this.Network);
        }

        private void InitializeAutoSave()
        {
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(this.AutoSaveDuration)
            };

            timer.Tick += (o, e2) =>
            {
                if (this.IsAutoSaveEnabled && this.Graph != null)
                {
                    try
                    {
                        this.Graph.Write(this.Network);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        var fileName = this.Network.FileName;

                        var fileInfo = new FileInfo(fileName);

                        var message = fileInfo.IsReadOnly ? "File is read only. Cannot save to " + fileName : "Cannot access " + fileName;

                        var notification = new Notification
                        {
                            IsTimed = true,
                            Description = message
                        };

                        this.RaiseNotificationOpened(notification);
                    }
                }
            };

            timer.Start();
        }

        private void Open()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = @"Network Files|*.net|Binary Network Files|*.enet",
                FilterIndex = 1,
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            this.Open(openFileDialog.FileName);
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            this.Open();
        }

        private void RadDiagramShape_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var radDiagramShape = sender as RadDiagramShape;

            this.Graph.SelectedVertex = radDiagramShape.DataContext as Vertex;
            this.BringShapeToFront(radDiagramShape);
        }

        private void RaiseEvidenceEntered(VertexEvidence vertexEvidence = null)
        {
            if (this.EvidenceEntered != null)
            {
                this.EvidenceEntered(this, vertexEvidence);
            }
        }

        private void RaiseGraphChanged(Graph newGraph, Graph oldGraph)
        {
            if (this.GraphChanged != null)
            {
                this.GraphChanged(this,
                    new ValueChangedEventArgs<Graph>
                    {
                        NewValue = newGraph,
                        OldValue = oldGraph
                    });
            }
        }

        private void RaiseNotificationOpened(Notification notification)
        {
            if (this.NotificationOpened != null)
            {
                this.NotificationOpened(this, notification);
            }
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null && propertyName != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void RaiseSelectionChanged(Vertex vertex)
        {
            if (this.SelectionChanged != null)
            {
                this.SelectionChanged(this, vertex);
            }
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Graph != null)
            {
                this.Graph.Belief = this.Network.Run(this.Graph.Evidence);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.Graph.Write(this.Network);
        }

        private void SaveEvidenceButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new SaveFileDialog
            {
                Filter = @"Hugin Case|*.hcs|MARV Network Evidence|*.marv-networkevidence",
                FilterIndex = 1,
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            this.WriteEvidences(openFileDialog.FileName);
        }

        private void UpdateDisplayGraph(string group, string vertexKey = null)
        {
            if (vertexKey == null)
            {
                vertexKey = this.displayVertexKey;
            }

            this.DisplayGraph = this.Graph.GetSubGraph(group, vertexKey);
            this.IsDefaultGroupVisible = @group == this.Graph.DefaultGroup;

            this.displayVertexKey = vertexKey;
        }

        private void UpdateLayout(bool isAutoFitDone = false, bool isAsync = true)
        {
            if (this.IsAutoLayoutEnabled)
            {
                if (isAutoFitDone)
                {
                    this.Diagram.DiagramLayoutComplete -= this.Diagram_DiagramLayoutComplete;
                    this.Diagram.DiagramLayoutComplete += this.Diagram_DiagramLayoutComplete;
                }

                var sugiyamaSettings = new SugiyamaSettings
                {
                    AnimateTransitions = true,
                    Orientation = Orientation.Vertical,
                    VerticalDistance = 128,
                    HorizontalDistance = 128,
                    ComponentMargin = new Size(128, 128)
                };

                if (isAsync)
                {
                    this.Diagram.LayoutAsync(LayoutType.Sugiyama, sugiyamaSettings);
                }
                else
                {
                    this.Diagram.Layout(LayoutType.Sugiyama, sugiyamaSettings);
                }

                this.Diagram.InvalidateVisual();
            }
            else
            {
                if (isAutoFitDone)
                {
                    this.Diagram.AutoFitAsync(new Thickness(10));
                }
            }
        }

        private void WriteEvidences(string filePath)
        {
            if (Path.GetExtension(filePath) == ".hcs")
            {
                this.Graph.Evidence.WriteHcs(filePath);
                return;
            }

            this.Graph.Evidence.WriteJson(filePath);
        }

        public event EventHandler<VertexEvidence> EvidenceEntered;
        public event EventHandler<ValueChangedEventArgs<Graph>> GraphChanged;
        public event EventHandler<Notification> NotificationClosed;
        public event EventHandler<Notification> NotificationOpened;
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<Vertex> SelectionChanged;
    }
}