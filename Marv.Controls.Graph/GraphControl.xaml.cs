using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using NLog;

namespace Marv.Controls.Graph
{
    public partial class GraphControl : UserControl
    {
        public static readonly DependencyProperty ConnectionColorProperty =
        DependencyProperty.Register("ConnectionColor", typeof(Color), typeof(GraphControl), new PropertyMetadata(Colors.LightSlateGray));

        public static readonly DependencyProperty GraphProperty =
        DependencyProperty.Register("Graph", typeof(Common.Graph.Graph), typeof(GraphControl), new PropertyMetadata(null));

        public static readonly DependencyProperty IncomingConnectionHighlightColorProperty =
        DependencyProperty.Register("IncomingConnectionHighlightColor", typeof(Color), typeof(GraphControl), new PropertyMetadata(Colors.SkyBlue));

        public static readonly DependencyProperty OutgoingConnectionHighlightColorProperty =
        DependencyProperty.Register("OutgoingConnectionHighlightColor", typeof(Color), typeof(GraphControl), new PropertyMetadata(Colors.Red));

        public static readonly DependencyProperty ShapeOpacityProperty =
        DependencyProperty.Register("ShapeOpacity", typeof(double), typeof(GraphControl), new PropertyMetadata(1.0));

        public static readonly RoutedEvent StateDoubleClickedEvent =
        EventManager.RegisterRoutedEvent("StateDoubleClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler<GraphControlEventArgs>), typeof(GraphControl));

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public GraphControl()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler<GraphControlEventArgs> StateDoubleClicked
        {
            add { this.AddHandler(StateDoubleClickedEvent, value); }
            remove { this.RemoveHandler(StateDoubleClickedEvent, value); }
        }

        public Color ConnectionColor
        {
            get { return (Color)this.GetValue(ConnectionColorProperty); }
            set { this.SetValue(ConnectionColorProperty, value); }
        }

        public Common.Graph.Graph Graph
        {
            get { return (Common.Graph.Graph)this.GetValue(GraphProperty); }
            set { this.SetValue(GraphProperty, value); }
        }

        public Color IncomingConnectionHighlightColor
        {
            get { return (Color)this.GetValue(IncomingConnectionHighlightColorProperty); }
            set { this.SetValue(IncomingConnectionHighlightColorProperty, value); }
        }

        public Color OutgoingConnectionHighlightColor
        {
            get { return (Color)this.GetValue(OutgoingConnectionHighlightColorProperty); }
            set { this.SetValue(OutgoingConnectionHighlightColorProperty, value); }
        }

        public double ShapeOpacity
        {
            get { return (double)this.GetValue(ShapeOpacityProperty); }
            set { this.SetValue(ShapeOpacityProperty, value); }
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
    }
}