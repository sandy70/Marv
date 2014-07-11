using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;
using Marv.Common;
using Marv.Common.Graph;

namespace Marv.Controls.Graph
{
    public partial class GraphControl
    {
        public static readonly DependencyProperty ConnectionColorProperty =
            DependencyProperty.Register("ConnectionColor", typeof (Color), typeof (GraphControl), new PropertyMetadata(Colors.LightSlateGray));

        public static readonly DependencyProperty GraphProperty =
            DependencyProperty.Register("Graph", typeof (Common.Graph.Graph), typeof (GraphControl), new PropertyMetadata(null));

        public static readonly DependencyProperty IncomingConnectionHighlightColorProperty =
            DependencyProperty.Register("IncomingConnectionHighlightColor", typeof (Color), typeof (GraphControl), new PropertyMetadata(Colors.SkyBlue));

        public static readonly DependencyProperty IsInputVisibleProperty =
            DependencyProperty.Register("IsInputVisible", typeof (bool), typeof (GraphControl), new PropertyMetadata(false));

        public static readonly DependencyProperty IsVerticesEnabledProperty =
            DependencyProperty.Register("IsVerticesEnabled", typeof (bool), typeof (GraphControl), new PropertyMetadata(true));

        public static readonly DependencyProperty OutgoingConnectionHighlightColorProperty =
            DependencyProperty.Register("OutgoingConnectionHighlightColor", typeof (Color), typeof (GraphControl), new PropertyMetadata(Colors.Red));

        public static readonly DependencyProperty SelectedVertexProperty =
            DependencyProperty.Register("SelectedVertex", typeof (Vertex), typeof (GraphControl), new PropertyMetadata(null));

        public static readonly DependencyProperty ShapeOpacityProperty =
            DependencyProperty.Register("ShapeOpacity", typeof (double), typeof (GraphControl), new PropertyMetadata(1.0));

        public static readonly DependencyProperty AutoSaveDurationProperty =
            DependencyProperty.Register("AutoSaveDuration", typeof(int), typeof(GraphControl), new PropertyMetadata(10000));

        public static readonly DependencyProperty IsAutoSaveEnabledProperty =
            DependencyProperty.Register("IsAutoSaveEnabled", typeof(bool), typeof(GraphControl), new PropertyMetadata(true));

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

        public Vertex SelectedVertex
        {
            get
            {
                return (Vertex) GetValue(SelectedVertexProperty);
            }

            set
            {
                SetValue(SelectedVertexProperty, value);
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

        public int AutoSaveDuration
        {
            get
            {
                return (int)this.GetValue(AutoSaveDurationProperty);
            }
            set
            {
                this.SetValue(AutoSaveDurationProperty, value);
            }
        }

        public bool IsAutoSaveEnabled
        {
            get
            {
                return (bool)this.GetValue(IsAutoSaveEnabledProperty);
            }
            set
            {
                this.SetValue(IsAutoSaveEnabledProperty, value);
            }
        }

        public GraphControl()
        {
            InitializeComponent();
            InitializeAutoSave();
            this.Loaded += GraphControl_Loaded;
        }


        public void InitializeAutoSave()
        {
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(AutoSaveDuration)
            };

            timer.Tick += (o, e2) =>
            {
                if (!IsAutoSaveEnabled)
                {
                    timer.Stop();
                }
                else
                {
                    this.Graph.Write(this.Graph.FileName);
                }
            };
            timer.Start();
        }

        public void AutoFit()
        {
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
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

        public void RaiseEvidenceEntered(Vertex vertex = null)
        {
            if (this.EvidenceEntered != null)
            {
                this.EvidenceEntered(this, vertex);
            }
        }

        public void RaiseVertexCommandExecuted(Vertex vertex, Command<Vertex> command)
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Graph.UpdateDisplayGraph(this.Graph.DefaultGroup);
        }

        private void ClearEvidenceButton_Click(object sender, RoutedEventArgs e)
        {
            this.Graph.Evidence = null;
            this.RaiseEvidenceEntered();
        }

        private void DiagramPart_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                this.RaiseSelectionChanged(e.AddedItems[0] as Vertex);
            }
        }

        private void ExpandButton_Click(object sender, RoutedEventArgs e)
        {
            this.Graph.DisplayGraph.IsExpanded = !this.Graph.DisplayGraph.IsMostlyExpanded;
        }

        private void GraphControl_Loaded(object sender, RoutedEventArgs e)
        {
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

            this.DiagramPart.SelectionChanged -= DiagramPart_SelectionChanged;
            this.DiagramPart.SelectionChanged += DiagramPart_SelectionChanged;
        }

        private void OpenNetworkButton_Click(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog();

            openDialog.Filter = "Network Files (.net)|*.net";
            openDialog.FilterIndex = 1;
            openDialog.Multiselect = false;

            if (openDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            this.Graph = Common.Graph.Graph.Read(openDialog.FileName);
            this.Graph.Run();
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
            this.Graph.Run();
        }

        private void SaveNetworkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Graph.Write(this.Graph.FileName);
        }

        public event EventHandler<Vertex> EvidenceEntered;

        public event EventHandler<VertexCommandArgs> VertexCommandExecuted;

        public event EventHandler<Vertex> SelectionChanged;
    }
}