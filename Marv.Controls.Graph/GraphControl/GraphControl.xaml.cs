using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Marv.Common;
using Marv.Common.Graph;
using NLog;

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

        public static readonly DependencyProperty OutgoingConnectionHighlightColorProperty =
            DependencyProperty.Register("OutgoingConnectionHighlightColor", typeof (Color), typeof (GraphControl), new PropertyMetadata(Colors.Red));

        public static readonly DependencyProperty ShapeOpacityProperty =
            DependencyProperty.Register("ShapeOpacity", typeof (double), typeof (GraphControl), new PropertyMetadata(1.0));

        public static readonly DependencyProperty IsInputVisibleProperty =
            DependencyProperty.Register("IsInputVisible", typeof (bool), typeof (GraphControl), new PropertyMetadata(false));

        public static readonly DependencyProperty SelectedVertexProperty =
            DependencyProperty.Register("SelectedVertex", typeof (Vertex), typeof (GraphControl), new PropertyMetadata(null));

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public GraphControl()
        {
            InitializeComponent();

            this.Loaded += GraphControl_Loaded;
        }

        void GraphControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.BackButton.Click += BackButton_Click;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Graph.DisplayGraph = this.Graph.GetSubGraph(this.Graph.DefaultGroup);
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

        public void RaiseEvidenceEntered(Vertex vertex)
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

        public event EventHandler<Vertex> EvidenceEntered;

        public event EventHandler<VertexCommandArgs> VertexCommandExecuted;
    }
}