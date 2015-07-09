using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Marv.Common;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Diagrams;
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

        public static readonly DependencyProperty IncomingConnectionHighlightColorProperty =
            DependencyProperty.Register("IncomingConnectionHighlightColor", typeof (Color), typeof (GraphControl), new PropertyMetadata(Colors.SkyBlue));

        public static readonly DependencyProperty IsAdvancedToolbarVisibleProperty =
            DependencyProperty.Register("IsAdvancedToolbarVisible", typeof (bool), typeof (GraphControl), new PropertyMetadata(true));

        public static readonly DependencyProperty IsAutoLayoutEnabledProperty =
            DependencyProperty.Register("IsAutoLayoutEnabled", typeof (bool), typeof (GraphControl), new PropertyMetadata(false));

        public static readonly DependencyProperty IsAutoRunEnabledProperty =
            DependencyProperty.Register("IsAutoRunEnabled", typeof (bool), typeof (GraphControl), new PropertyMetadata(false));

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

        public static readonly DependencyProperty SelectedVertexProperty =
            DependencyProperty.Register("SelectedVertex", typeof (Vertex), typeof (GraphControl), new PropertyMetadata(null));

        public static readonly DependencyProperty ShapeOpacityProperty =
            DependencyProperty.Register("ShapeOpacity", typeof (double), typeof (GraphControl), new PropertyMetadata(1.0));

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof (string), typeof (GraphControl), new PropertyMetadata(null));

        private Graph displayGraph;
        private Graph graph;
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
            get { return this.graph; }

            set
            {
                if (value.Equals(this.graph))
                {
                    return;
                }

                this.graph = value;
                this.RaisePropertyChanged();
            }
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

        public bool IsAutoRunEnabled
        {
            get { return (bool) GetValue(IsAutoRunEnabledProperty); }
            set { SetValue(IsAutoRunEnabledProperty, value); }
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

        public Vertex SelectedVertex
        {
            get { return (Vertex) GetValue(SelectedVertexProperty); }
            set { SetValue(SelectedVertexProperty, value); }
        }

        public double ShapeOpacity
        {
            get { return (double) this.GetValue(ShapeOpacityProperty); }

            set { this.SetValue(ShapeOpacityProperty, value); }
        }

        public string Source
        {
            get { return (string) GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public GraphControl()
        {
            InitializeComponent();
            this.InitializeAutoSave();
        }

        public void Open(string fileName)
        {
            this.Network = Network.Read(fileName);

            this.Graph = Graph.Read(this.Network);
            this.Graph.Belief = this.Network.GetBeliefs();
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
            this.UpdateDisplayGraph();
        }

        private void BringIntoView(RadDiagramItem shape)
        {
            // Bring shape to view.
            // We cannot use the default BringIntoView() becuase the shape is only partially obscured

            // If shape is within viewport, do nothing.
            if (shape.Bounds.IsInBounds(this.Diagram.Viewport))
            {
                return;
            }

            var offset = this.Diagram.Viewport.GetOffset(shape.Bounds, 20);

            // Extension OffsetRect is part of Telerik.Windows.Diagrams.Core
            this.Diagram.BringIntoView(this.Diagram.Viewport.OffsetRect(offset.X, offset.Y));

            var animation = new DoubleAnimation
            {
                AutoReverse = true,
                Duration = TimeSpan.FromMilliseconds(300),
                From = 1,
                To = 0.3,
                RepeatBehavior = new RepeatBehavior(3)
            };

            shape.BeginAnimation(OpacityProperty, animation);
        }

        private void BringToFront(IShape shape)
        {
            // Bring shape to front
            this.Diagram.BringToFront(shape.Yield());

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

        private async void GraphControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.Source))
            {
                var notification = new Notification
                {
                    IsIndeterminate = true,
                    Description = "Opening network..."
                };

                this.RaiseNotificationOpened(notification);

                await this.OpenAsync(this.Source);

                this.RaiseNotificationClosed(notification);
            }
        }

        private void GraphControl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Graph.Write(this.Network);
        }

        private void GroupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.UpdateDisplayGraph();
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

        private async Task OpenAsync(string fileName)
        {
            this.Network = await Task.Run(() => Network.Read(fileName));

            var network = this.Network;
            this.Graph = await Task.Run(() => Graph.Read(network));
            this.Graph.Belief = this.Network.GetBeliefs();

            this.SelectedGroup = this.Graph.DefaultGroup;
        }

        private async void OpenButton_Click(object sender, RoutedEventArgs e)
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

            var notification = new Notification
            {
                IsIndeterminate = true,
                Description = "Opening network..."
            };

            this.RaiseNotificationOpened(notification);

            await this.OpenAsync(openFileDialog.FileName);

            this.RaiseNotificationClosed(notification);
        }

        private void RadDiagramShape_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var radDiagramShape = sender as RadDiagramShape;

            this.BringIntoView(radDiagramShape);
            this.BringToFront(radDiagramShape);

            this.SelectedVertex = radDiagramShape.DataContext as Vertex;
        }

        private void RaiseEvidenceEntered(VertexEvidence vertexEvidence = null)
        {
            if (this.EvidenceEntered != null)
            {
                this.EvidenceEntered(this, vertexEvidence);
            }

            if (!this.IsAutoRunEnabled)
            {
                return;
            }

            if (this.Graph != null)
            {
                this.Graph.Belief = this.Network.Run(this.Graph.Evidence);
            }
        }

        private void RaiseNotificationClosed(Notification notification)
        {
            if (this.NotificationClosed != null)
            {
                this.NotificationClosed(this, notification);
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

        private void UpdateDisplayGraph()
        {
            this.DisplayGraph = this.Graph.GetSubGraph(this.SelectedGroup);
            this.IsDefaultGroupVisible = this.SelectedGroup == this.Graph.DefaultGroup;

            if (!this.DisplayGraph.ContainsVertex(this.SelectedVertex))
            {
                this.SelectedVertex = this.DisplayGraph.GetSink();
            }
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

        private void VertexComboxBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SelectedVertex == null)
            {
                return;
            }

            if (!this.SelectedVertex.Groups.Contains(this.SelectedGroup))
            {
                this.SelectedGroup = this.SelectedVertex.Groups.Count > 1
                                         ? this.SelectedVertex.Groups.Except("all").First()
                                         : this.SelectedVertex.Groups.First();

                this.UpdateDisplayGraph();
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
        public event EventHandler<Notification> NotificationClosed;
        public event EventHandler<Notification> NotificationOpened;
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<Vertex> SelectionChanged;

        private void GroupsCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SelectedVertex = this.DisplayGraph.GetSink();
        }
    }
}