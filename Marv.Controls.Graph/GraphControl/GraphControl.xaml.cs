﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;
using Marv.Common;
using Marv.Common.Graph;

namespace Marv.Controls.Graph
{
    public partial class GraphControl
    {
        public static readonly DependencyProperty AutoSaveDurationProperty =
            DependencyProperty.Register("AutoSaveDuration", typeof (int), typeof (GraphControl), new PropertyMetadata(10000));

        public static readonly DependencyProperty ConnectionColorProperty =
            DependencyProperty.Register("ConnectionColor", typeof (Color), typeof (GraphControl), new PropertyMetadata(Colors.LightSlateGray));

        public static readonly DependencyProperty GraphProperty =
            DependencyProperty.Register("Graph", typeof (Common.Graph.Graph), typeof (GraphControl), new PropertyMetadata(null, ChangedGraph));

        public static readonly DependencyProperty IncomingConnectionHighlightColorProperty =
            DependencyProperty.Register("IncomingConnectionHighlightColor", typeof (Color), typeof (GraphControl), new PropertyMetadata(Colors.SkyBlue));

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

        public int AutoSaveDuration
        {
            get
            {
                return (int) this.GetValue(AutoSaveDurationProperty);
            }

            set
            {
                this.SetValue(AutoSaveDurationProperty, value);
            }
        }

        public Color ConnectionColor
        {
            get
            {
                return (Color) this.GetValue(ConnectionColorProperty);
            }

            set
            {
                this.SetValue(ConnectionColorProperty, value);
            }
        }

        public Common.Graph.Graph Graph
        {
            get
            {
                return (Common.Graph.Graph) this.GetValue(GraphProperty);
            }

            set
            {
                this.SetValue(GraphProperty, value);
            }
        }

        public Color IncomingConnectionHighlightColor
        {
            get
            {
                return (Color) this.GetValue(IncomingConnectionHighlightColorProperty);
            }

            set
            {
                this.SetValue(IncomingConnectionHighlightColorProperty, value);
            }
        }

        public bool IsAutoSaveEnabled
        {
            get
            {
                return (bool) this.GetValue(IsAutoSaveEnabledProperty);
            }

            set
            {
                this.SetValue(IsAutoSaveEnabledProperty, value);
            }
        }

        public bool IsInputVisible
        {
            get
            {
                return (bool) GetValue(IsInputVisibleProperty);
            }

            set
            {
                SetValue(IsInputVisibleProperty, value);
            }
        }

        public bool IsNavigationPaneVisible
        {
            get
            {
                return (bool) GetValue(IsNavigationPaneVisibleProperty);
            }
            set
            {
                SetValue(IsNavigationPaneVisibleProperty, value);
            }
        }

        public bool IsVerticesEnabled
        {
            get
            {
                return (bool) GetValue(IsVerticesEnabledProperty);
            }

            set
            {
                SetValue(IsVerticesEnabledProperty, value);
            }
        }

        public Color OutgoingConnectionHighlightColor
        {
            get
            {
                return (Color) this.GetValue(OutgoingConnectionHighlightColorProperty);
            }

            set
            {
                this.SetValue(OutgoingConnectionHighlightColorProperty, value);
            }
        }

        public double ShapeOpacity
        {
            get
            {
                return (double) this.GetValue(ShapeOpacityProperty);
            }

            set
            {
                this.SetValue(ShapeOpacityProperty, value);
            }
        }

        public GraphControl()
        {
            InitializeComponent();
            InitializeAutoSave();

            this.Loaded += GraphControl_Loaded;
        }

        private static void ChangedGraph(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as GraphControl;

            if (control == null || control.Graph == null) return;

            var oldGraph = e.OldValue as Common.Graph.Graph;

            control.RaiseGraphChanged(e.NewValue as Common.Graph.Graph, oldGraph);

            if (oldGraph != null)
            {
                oldGraph.PropertyChanged -= control.Graph_PropertyChanged;
            }

            control.Graph.PropertyChanged -= control.Graph_PropertyChanged;
            control.Graph.PropertyChanged += control.Graph_PropertyChanged;

            if (control.Graph.Vertices.Count > 0)
            {
                control.Graph.SelectedVertex = control.Graph.Vertices[0];
            }
        }

        public void AutoFit()
        {
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300)
            };

            timer.Tick += (o, e2) =>
            {
                this.DiagramPart.AutoFit();
                timer.Stop();
            };

            timer.Start();
        }

        public void DisableConnectorEditing()
        {
            this.IsVerticesEnabled = true;
            this.DiagramPart.IsConnectorsManipulationEnabled = false;
            this.DiagramPart.IsManipulationAdornerVisible = false;
        }

        public void EnableConnectorEditing()
        {
            this.IsVerticesEnabled = false;
            this.DiagramPart.IsConnectorsManipulationEnabled = true;
            this.DiagramPart.IsManipulationAdornerVisible = true;
        }

        public void Graph_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedVertex")
            {
                this.RaiseSelectionChanged(this.Graph.SelectedVertex);
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
            this.Graph = Common.Graph.Graph.Read(fileName);
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

        public void RaiseGraphChanged(Common.Graph.Graph newGraph, Common.Graph.Graph oldGraph)
        {
            if (this.GraphChanged != null)
            {
                this.GraphChanged(this, new ValueChangedArgs<Common.Graph.Graph>
                {
                    NewValue = newGraph,
                    OldValue = oldGraph
                });
            }
        }

        internal void RaiseEvidenceEntered(Vertex vertex = null)
        {
            if (this.EvidenceEntered != null)
            {
                this.EvidenceEntered(this, vertex);
            }
        }

        internal void RaiseSelectionChanged(Vertex vertex)
        {
            if (this.SelectionChanged != null)
            {
                this.SelectionChanged(this, vertex);
            }
        }

        internal void RaiseVertexCommandExecuted(Vertex vertex, Command<Vertex> command)
        {
            if (this.VertexCommandExecuted != null)
            {
                this.VertexCommandExecuted(this, new VertexCommandArgs
                {
                    Command = command,
                    Vertex = vertex
                });
            }
        }

        private void AutoFitButton_Click(object sender, RoutedEventArgs e)
        {
            this.AutoFit();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Graph.UpdateDisplayGraph(this.Graph.DefaultGroup);
        }

        private void ClearEvidenceButton_Click(object sender, RoutedEventArgs e)
        {
            this.Graph.Evidence = null;
            this.RaiseEvidenceEntered();
        }

        private void ExpandButton_Click(object sender, RoutedEventArgs e)
        {
            this.Graph.DisplayGraph.IsExpanded = !this.Graph.DisplayGraph.IsMostlyExpanded;
        }

        private void GraphControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.AutoFitButton.Click -= AutoFitButton_Click;
            this.AutoFitButton.Click += AutoFitButton_Click;

            this.BackButton.Click -= BackButton_Click;
            this.BackButton.Click += BackButton_Click;

            this.ClearEvidenceButton.Click -= this.ClearEvidenceButton_Click;
            this.ClearEvidenceButton.Click += this.ClearEvidenceButton_Click;

            this.ExpandButton.Click -= ExpandButton_Click;
            this.ExpandButton.Click += ExpandButton_Click;

            this.RunButton.Click -= RunButton_Click;
            this.RunButton.Click += RunButton_Click;

            this.OpenNetworkButton.Click -= OpenNetworkButton_Click;
            this.OpenNetworkButton.Click += OpenNetworkButton_Click;

            this.SaveNetworkButton.Click -= SaveNetworkButton_Click;
            this.SaveNetworkButton.Click += SaveNetworkButton_Click;
        }

        private void OpenNetworkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Open();
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            this.Graph.Run();
        }

        private void SaveNetworkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Graph.Write();
        }

        public event EventHandler<Vertex> EvidenceEntered;

        public event EventHandler<VertexCommandArgs> VertexCommandExecuted;

        public event EventHandler<Vertex> SelectionChanged;

        public event EventHandler<ValueChangedArgs<Common.Graph.Graph>> GraphChanged;
    }
}