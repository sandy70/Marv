using Marv.Common;
using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Linq;
using NLog;
using System.Collections.Generic;

namespace Marv.Controls
{
    public partial class BnGraphControl : UserControl
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static readonly DependencyProperty ConnectionColorProperty =
        DependencyProperty.Register("ConnectionColor", typeof(Color), typeof(BnGraphControl), new PropertyMetadata(Colors.LightSlateGray));

        public static readonly DependencyProperty GraphProperty =
        DependencyProperty.Register("Graph", typeof(Graph), typeof(BnGraphControl), new PropertyMetadata(null));

        public static readonly DependencyProperty IncomingConnectionHighlightColorProperty =
        DependencyProperty.Register("IncomingConnectionHighlightColor", typeof(Color), typeof(BnGraphControl), new PropertyMetadata(Colors.SkyBlue));

        public static readonly DependencyProperty OutgoingConnectionHighlightColorProperty =
        DependencyProperty.Register("OutgoingConnectionHighlightColor", typeof(Color), typeof(BnGraphControl), new PropertyMetadata(Colors.Red));

        public static readonly DependencyProperty ShapeOpacityProperty =
        DependencyProperty.Register("ShapeOpacity", typeof(double), typeof(BnGraphControl), new PropertyMetadata(1.0));

        public static readonly DependencyProperty SourceConnectorPositionProperty =
        DependencyProperty.Register("SourceConnectorPosition", typeof(string), typeof(BnGraphControl), new PropertyMetadata("Bottom", ChangedSourceConnectorPosition));

        public static readonly RoutedEvent StateDoubleClickedEvent =
        EventManager.RegisterRoutedEvent("StateDoubleClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler<BnGraphControlEventArgs>), typeof(BnGraphControl));

        public static readonly DependencyProperty TargetConnectorPositionProperty =
        DependencyProperty.Register("TargetConnectorPosition", typeof(string), typeof(BnGraphControl), new PropertyMetadata("Top", ChangedTargetConnectorPosition));

        public BnGraphControl()
        {
            InitializeComponent();
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

        public Graph Graph
        {
            get { return (Graph)GetValue(GraphProperty); }
            set { SetValue(GraphProperty, value); }
        }

        public Color IncomingConnectionHighlightColor
        {
            get { return (Color)GetValue(IncomingConnectionHighlightColorProperty); }
            set { SetValue(IncomingConnectionHighlightColorProperty, value); }
        }

        public Color OutgoingConnectionHighlightColor
        {
            get { return (Color)GetValue(OutgoingConnectionHighlightColorProperty); }
            set { SetValue(OutgoingConnectionHighlightColorProperty, value); }
        }

        public double ShapeOpacity
        {
            get { return (double)GetValue(ShapeOpacityProperty); }
            set { SetValue(ShapeOpacityProperty, value); }
        }

        public string SourceConnectorPosition
        {
            get { return (string)GetValue(SourceConnectorPositionProperty); }
            set { SetValue(SourceConnectorPositionProperty, value); }
        }

        public string TargetConnectorPosition
        {
            get { return (string)GetValue(TargetConnectorPositionProperty); }
            set { SetValue(TargetConnectorPositionProperty, value); }
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

        private static void ChangedSourceConnectorPosition(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as BnGraphControl;

            if (control.Graph != null)
            {
                control.Graph.SourceConnectorPosition = control.SourceConnectorPosition;

                foreach (var vertex in control.Graph.Vertices)
                {
                    var displayPosition = vertex.DisplayPosition;
                    displayPosition.X += 1;
                    displayPosition.X -= 1;
                }
            }
        }

        private static void ChangedTargetConnectorPosition(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as BnGraphControl;

            if (control.Graph != null)
            {
                control.Graph.TargetConnectorPosition = control.TargetConnectorPosition;

                foreach (var vertex in control.Graph.Vertices)
                {
                    var displayPosition = vertex.DisplayPosition;
                    displayPosition.X += 1;
                    displayPosition.X -= 1;
                }
            }
        }
    }
}