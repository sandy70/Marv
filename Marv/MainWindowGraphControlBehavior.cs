using LibNetwork;
using LibPipeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media.Animation;

namespace Marv
{
    internal class MainWindowGraphControlBehavior : Behavior<MainWindow>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            var window = this.AssociatedObject;

            window.GraphControl.BackButtonClicked += GraphControl_BackButtonClicked;
            window.GraphControl.GroupButtonClicked += GraphControl_GroupButtonClicked;
            window.GraphControl.NewEvidenceAvailable += GraphControl_NewEvidenceAvailable;
            window.GraphControl.RetractButtonClicked += GraphControl_RetractButtonClicked;
            window.GraphControl.SensorButtonChecked += GraphControl_SensorButtonChecked;
            window.GraphControl.SensorButtonUnchecked += GraphControl_SensorButtonUnchecked;
            window.GraphControl.StateDoubleClicked += GraphControl_StateDoubleClicked;
        }

        private void GraphControl_BackButtonClicked(object sender, ValueEventArgs<BnVertexViewModel> e)
        {
            var window = this.AssociatedObject;
            var vertex = e.Value;

            window.DisplayGraph = window.SourceGraph.GetSubGraph(window.SourceGraph.DefaultGroup);
            window.IsBackButtonVisible = false;
        }

        private void GraphControl_GroupButtonClicked(object sender, ValueEventArgs<BnVertexViewModel> e)
        {
            var window = this.AssociatedObject;
            var vertex = e.Value;

            window.DisplayGraph = vertex.GetSubGraph();
            window.IsBackButtonVisible = true;
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
                    window.PopupControl.ShowText("Inconsistent evidence entered.");
                    graph.Value = graph.ClearEvidence(vertex.Key);
                }
            }
            else
            {
                graph.Value = graph.ClearEvidence(vertex.Key);
            }
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
            catch(InconsistentEvidenceException exception)
            {
                window.PopupControl.ShowText("Inconsistent evidence entered.");
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
            try
            {
                this.AssociatedObject.SensorListener.Start(e.Value);
            }
            catch (IOException exp)
            {
                this.AssociatedObject.SensorListener.Stop();
                this.AssociatedObject.PopupControl.ShowText("Unable to open serial port. Connect receiver.");
            }
        }

        private void GraphControl_SensorButtonUnchecked(object sender, ValueEventArgs<BnVertexViewModel> e)
        {
            this.AssociatedObject.SensorListener.Stop();
        }
    }
}