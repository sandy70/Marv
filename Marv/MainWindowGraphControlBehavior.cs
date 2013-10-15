using LibNetwork;
using LibPipeline;
using Marv.Common;
using Marv.Controls;
using NLog;
using System.IO;
using System.Windows;
using System.Windows.Interactivity;

namespace Marv
{
    internal class MainWindowGraphControlBehavior : Behavior<MainWindow>
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            var window = this.AssociatedObject;

            window.GraphControl.GroupButtonClicked += GraphControl_GroupButtonClicked;
            window.GraphControl.NewEvidenceAvailable += GraphControl_NewEvidenceAvailable;
            window.GraphControl.RetractButtonClicked += GraphControl_RetractButtonClicked;
            window.GraphControl.SensorButtonChecked += GraphControl_SensorButtonChecked;
            window.GraphControl.SensorButtonUnchecked += GraphControl_SensorButtonUnchecked;
            window.GraphControl.StateDoubleClicked += GraphControl_StateDoubleClicked;
        }

        private void GraphControl_GroupButtonClicked(object sender, ValueEventArgs<BnVertexViewModel> e)
        {
            var window = this.AssociatedObject;
            var displayGraph = window.DisplayGraph;
            var vertex = e.Value;
            var sourceGraph = window.SourceGraph;

            window.DisplayGraph = sourceGraph.GetSubGraph(vertex.HeaderOfGroup);
            window.IsBackButtonVisible = true;
        }

        private void GraphControl_NewEvidenceAvailable(object sender, ValueEventArgs<BnVertexViewModel> e)
        {
            var window = this.AssociatedObject;
            var graph = window.SourceGraph;
            var vertex = e.Value;

            try
            {
                graph.Value = graph.Run(vertex.Key, vertex.ToEvidence());
            }
            catch (InconsistentEvidenceException exception)
            {
                window.Notifications.Push(new TimedNotification
                {
                    Name = "Inconsistent Evidence",
                    Description = "Inconsistent evidence entered for vertex: " + vertex.Name,
                });

                graph.Value = graph.ClearEvidence(vertex.Key);
            }
        }

        private void GraphControl_RetractButtonClicked(object sender, ValueEventArgs<BnVertexViewModel> e)
        {
            var graph = this.AssociatedObject.SourceGraph;
            var vertex = e.Value;
            graph.Value = graph.ClearEvidence(vertex.Key);
        }

        private void GraphControl_SensorButtonChecked(object sender, ValueEventArgs<BnVertexViewModel> e)
        {
            var window = this.AssociatedObject;

            try
            {
                this.AssociatedObject.SensorListener.Start(e.Value);
            }
            catch (IOException exp)
            {
                this.AssociatedObject.SensorListener.Stop();

                window.Notifications.Push(new IndeterminateNotification
                {
                    Name = "Serial Port Error",
                    Description = "Unable to open serial port. Connect receiver and retry."
                });
            }
        }

        private void GraphControl_SensorButtonUnchecked(object sender, ValueEventArgs<BnVertexViewModel> e)
        {
            this.AssociatedObject.SensorListener.Stop();
        }

        private void GraphControl_StateDoubleClicked(object sender, BnGraphControlEventArgs e)
        {
            var window = this.AssociatedObject;
            var graph = window.SourceGraph;
            var vertex = e.Vertex;

            if (e.Vertex.SelectedState != e.State)
            {
                var evidence = new HardEvidence
                {
                    StateIndex = vertex.States.IndexOf(e.State)
                };

                try
                {
                    graph.Value = graph.Run(vertex.Key, evidence);
                }
                catch (InconsistentEvidenceException exception)
                {
                    window.Notifications.Push(new TimedNotification
                    {
                        Name = "Inconsistent Evidence",
                        Description = "Inconsistent evidence entered for vertex: " + vertex.Name,
                    });

                    graph.Value = graph.ClearEvidence(vertex.Key);
                }
            }
            else
            {
                graph.Value = graph.ClearEvidence(vertex.Key);
            }
        }
    }
}