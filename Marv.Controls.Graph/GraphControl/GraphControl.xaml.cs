using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;
using Telerik.Windows.Diagrams.Core;
using Orientation = Telerik.Windows.Diagrams.Core.Orientation;

namespace Marv.Controls.Graph
{
    public partial class GraphControl : INotifyPropertyChanged
    {
        public static readonly DependencyProperty AutoSaveDurationProperty =
            DependencyProperty.Register("AutoSaveDuration", typeof (int), typeof (GraphControl), new PropertyMetadata(10000));

        public static readonly DependencyProperty ConnectionColorProperty =
            DependencyProperty.Register("ConnectionColor", typeof (Color), typeof (GraphControl), new PropertyMetadata(Colors.LightSlateGray));

        public static readonly DependencyProperty GraphProperty =
            DependencyProperty.Register("Graph", typeof (Marv.Graph), typeof (GraphControl), new PropertyMetadata(null, ChangedGraph));

        public static readonly DependencyProperty IncomingConnectionHighlightColorProperty =
            DependencyProperty.Register("IncomingConnectionHighlightColor", typeof (Color), typeof (GraphControl), new PropertyMetadata(Colors.SkyBlue));

        public static readonly DependencyProperty IsAutoLayoutEnabledProperty =
            DependencyProperty.Register("IsAutoLayoutEnabled", typeof (bool), typeof (GraphControl), new PropertyMetadata(false, ChangedAutoLayoutEnabled));

        public static readonly DependencyProperty IsAutoRunEnabledProperty =
            DependencyProperty.Register("IsAutoRunEnabled", typeof (bool), typeof (GraphControl), new PropertyMetadata(false, ChangedAutoRunEnabled));

        public static readonly DependencyProperty IsAutoSaveEnabledProperty =
            DependencyProperty.Register("IsAutoSaveEnabled", typeof (bool), typeof (GraphControl), new PropertyMetadata(true));

        public static readonly DependencyProperty IsInputVisibleProperty =
            DependencyProperty.Register("IsInputVisible", typeof (bool), typeof (GraphControl), new PropertyMetadata(false));

        public static readonly DependencyProperty IsNavigationPaneVisibleProperty =
            DependencyProperty.Register("IsNavigationPaneVisible", typeof (bool), typeof (GraphControl), new PropertyMetadata(false));

        public static readonly DependencyProperty IsVerticesEnabledProperty =
            DependencyProperty.Register("IsVerticesEnabled", typeof (bool), typeof (GraphControl), new PropertyMetadata(true));

        public static readonly DependencyProperty OutgoingConnectionHighlightColorProperty =
            DependencyProperty.Register("OutgoingConnectionHighlightColor", typeof (Color), typeof (GraphControl), new PropertyMetadata(Colors.Red));

        public static readonly DependencyProperty ShapeOpacityProperty =
            DependencyProperty.Register("ShapeOpacity", typeof (double), typeof (GraphControl), new PropertyMetadata(1.0));

        private Marv.Graph displayGraph;
        private bool isDefaultGroupVisible;

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

        public Marv.Graph DisplayGraph
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

        public Marv.Graph Graph
        {
            get { return (Marv.Graph) this.GetValue(GraphProperty); }

            set { this.SetValue(GraphProperty, value); }
        }

        public Color IncomingConnectionHighlightColor
        {
            get { return (Color) this.GetValue(IncomingConnectionHighlightColorProperty); }

            set { this.SetValue(IncomingConnectionHighlightColorProperty, value); }
        }

        public bool IsAutoLayoutEnabled
        {
            get { return (bool) GetValue(IsAutoLayoutEnabledProperty); }
            set { SetValue(IsAutoLayoutEnabledProperty, value); }
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
            get { return (bool) GetValue(IsInputVisibleProperty); }

            set { SetValue(IsInputVisibleProperty, value); }
        }

        public bool IsNavigationPaneVisible
        {
            get { return (bool) GetValue(IsNavigationPaneVisibleProperty); }
            set { SetValue(IsNavigationPaneVisibleProperty, value); }
        }

        public bool IsVerticesEnabled
        {
            get { return (bool) GetValue(IsVerticesEnabledProperty); }

            set { SetValue(IsVerticesEnabledProperty, value); }
        }

        public Color OutgoingConnectionHighlightColor
        {
            get { return (Color) this.GetValue(OutgoingConnectionHighlightColorProperty); }

            set { this.SetValue(OutgoingConnectionHighlightColorProperty, value); }
        }

        public double ShapeOpacity
        {
            get { return (double) this.GetValue(ShapeOpacityProperty); }

            set { this.SetValue(ShapeOpacityProperty, value); }
        }

        public GraphControl()
        {
            InitializeComponent();
            InitializeAutoSave();

            this.Loaded += GraphControl_Loaded;
        }

        private static void ChangedAutoLayoutEnabled(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as GraphControl;

            control.UpdateLayout();

            if (control.IsAutoLayoutEnabled)
            {
                control.DisableVertexDragging();
            }
            else
            {
                control.EnableVertexDragging();
            }
        }

        private static void ChangedAutoRunEnabled(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as GraphControl).Run();
        }

        private static void ChangedGraph(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as GraphControl;

            if (control == null || control.Graph == null)
            {
                return;
            }

            var oldGraph = e.OldValue as Marv.Graph;

            control.RaiseGraphChanged(e.NewValue as Marv.Graph, oldGraph);

            if (control.Graph.Vertices.Count > 0)
            {
                control.Graph.SelectedVertex = control.Graph.Vertices[0];
            }

            control.UpdateDisplayGraph(control.Graph.DefaultGroup);
        }

        public void DisableConnectorEditing()
        {
            this.IsVerticesEnabled = true;
            this.DiagramPart.IsConnectorsManipulationEnabled = false;
            this.DiagramPart.IsManipulationAdornerVisible = false;
        }

        public void DisableVertexDragging()
        {
            foreach (var vertex in this.Graph.Vertices)
            {
                vertex.IsDraggingEnabled = false;
            }
        }

        public void EnableConnectorEditing()
        {
            this.IsVerticesEnabled = false;
            this.DiagramPart.IsConnectorsManipulationEnabled = true;
            this.DiagramPart.IsManipulationAdornerVisible = true;
        }

        public void EnableVertexDragging()
        {
            foreach (var vertex in this.Graph.Vertices)
            {
                vertex.IsDraggingEnabled = true;
            }
        }

        public void InitializeAutoSave()
        {
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(AutoSaveDuration)
            };

            timer.Tick += (o, e2) =>
            {
                if (this.IsAutoSaveEnabled && this.Graph != null)
                {
                    this.Graph.Write();
                }
            };

            timer.Start();
        }

        public void Open(string fileName)
        {
            this.Graph = Marv.Graph.Read(fileName);
            this.Graph.Run();
        }

        public void Open()
        {
            var openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Network Files (.net)|*.net";
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            this.Open(openFileDialog.FileName);
        }

        public void RaiseEvidenceEntered(VertexEvidence vertexEvidence = null)
        {
            if (this.EvidenceEntered != null)
            {
                this.EvidenceEntered(this, vertexEvidence);
            }
        }

        public void RaiseGraphChanged(Marv.Graph newGraph, Marv.Graph oldGraph)
        {
            if (this.GraphChanged != null)
            {
                this.GraphChanged(this, newGraph, oldGraph);
            }
        }

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null && propertyName != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void UpdateDisplayGraph(string group)
        {
            this.DisplayGraph = this.Graph.GetSubGraph(group);
            this.IsDefaultGroupVisible = @group == this.Graph.DefaultGroup;
        }

        public void UpdateLayout(bool isAutoFitDone = false, bool isAsync = true)
        {
            if (this.IsAutoLayoutEnabled)
            {
                if (isAutoFitDone)
                {
                    this.DiagramPart.DiagramLayoutComplete -= this.DiagramPart_DiagramLayoutComplete;
                    this.DiagramPart.DiagramLayoutComplete += this.DiagramPart_DiagramLayoutComplete;
                }

                var sugiyamaSettings = new SugiyamaSettings
                {
                    AnimateTransitions = true,
                    Orientation = Orientation.Vertical,
                    VerticalDistance = 128
                };

                if (isAsync)
                {
                    this.DiagramPart.LayoutAsync(LayoutType.Sugiyama, sugiyamaSettings);
                }
                else
                {
                    this.DiagramPart.Layout(LayoutType.Sugiyama, sugiyamaSettings);
                }
            }
            else
            {
                if (isAutoFitDone)
                {
                    this.DiagramPart.AutoFitAsync(new Thickness(10));
                }
            }
        }

        private void AutoFitButton_Click(object sender, RoutedEventArgs e)
        {
            this.DiagramPart.AutoFit(new Thickness(10));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateDisplayGraph(this.Graph.DefaultGroup);
        }

        private void ClearEvidenceButton_Click(object sender, RoutedEventArgs e)
        {
            this.Graph.Evidence = null;
            this.RaiseEvidenceEntered();
        }

        private void DiagramPart_DiagramLayoutComplete(object sender, RoutedEventArgs e)
        {
            this.DiagramPart.DiagramLayoutComplete -= DiagramPart_DiagramLayoutComplete;
            this.DiagramPart.AutoFitAsync(new Thickness(10));
        }

        private void DiagramPart_GraphSourceChanged(object sender, EventArgs e)
        {
            this.UpdateLayout(true);
        }

        private void ExpandButton_Click(object sender, RoutedEventArgs e)
        {
            this.DisplayGraph.IsExpanded = !this.DisplayGraph.IsMostlyExpanded;
            this.UpdateLayout();
        }

        private void GraphControl_EvidenceEntered(object sender, VertexEvidence e)
        {
            this.Run();
        }

        private void GraphControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.EvidenceEntered -= GraphControl_EvidenceEntered;
            this.EvidenceEntered += GraphControl_EvidenceEntered;

            // All the buttons
            this.AutoFitButton.Click -= AutoFitButton_Click;
            this.AutoFitButton.Click += AutoFitButton_Click;

            this.BackButton.Click -= BackButton_Click;
            this.BackButton.Click += BackButton_Click;

            this.ClearEvidenceButton.Click -= this.ClearEvidenceButton_Click;
            this.ClearEvidenceButton.Click += this.ClearEvidenceButton_Click;

            this.ExpandButton.Click -= ExpandButton_Click;
            this.ExpandButton.Click += ExpandButton_Click;

            this.OpenButton.Click -= this.OpenButton_Click;
            this.OpenButton.Click += this.OpenButton_Click;

            this.SaveButton.Click -= this.SaveButton_Click;
            this.SaveButton.Click += this.SaveButton_Click;

            // Other controls
            this.DiagramPart.GraphSourceChanged -= DiagramPart_GraphSourceChanged;
            this.DiagramPart.GraphSourceChanged += DiagramPart_GraphSourceChanged;
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            this.Open();
        }

        private void Run()
        {
            if (this.IsAutoRunEnabled && this.Graph != null)
            {
                Console.WriteLine("Running...");
                this.Graph.Run();
                Console.WriteLine("Run");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.Graph.Write();
        }

        public event EventHandler<VertexEvidence> EvidenceEntered;

        public event EventHandler<Marv.Graph, Marv.Graph> GraphChanged;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}