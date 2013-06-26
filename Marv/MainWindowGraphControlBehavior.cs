using LibBn;
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

            window.GraphControl.StateDoubleClicked += GraphControl_StateDoubleClicked;
            window.GraphControl.NewEvidenceAvailable += GraphControl_NewEvidenceAvailable;
            window.GraphControl.RetractButtonClicked += GraphControl_RetractButtonClicked;
            window.GraphControl.SensorButtonChecked += GraphControl_SensorButtonChecked;
            window.GraphControl.SensorButtonUnchecked += GraphControl_SensorButtonUnchecked;
        }

        private void GraphControl_StateDoubleClicked(object sender, BnGraphControlEventArgs e)
        {
            var window = this.AssociatedObject;

            var vertexEvidence = new VertexEvidence
            {
                EvidenceType = EvidenceType.StateSelected,
                StateIndex = e.Vertex.States.IndexOf(e.State)
            };

            try
            {
                e.Vertex.SetEvidence(vertexEvidence);
                e.Vertex.Parent.UpdateBeliefs();
                e.Vertex.Parent.UpdateValue();
            }
            catch (InconsistentEvidenceException exception)
            {
                window.PopupControl.ShowText("Inconsistent evidence entered.");
                e.Vertex.IsEvidenceEntered = false;
                e.Vertex.Parent.UpdateBeliefs();
                e.Vertex.Parent.UpdateValue();
            }
        }

        private void GraphControl_NewEvidenceAvailable(object sender, ValueEventArgs<BnVertexViewModel> e)
        {
            var window = this.AssociatedObject;
            var vertex = e.Value;

            var graphEvidence = new Dictionary<string, VertexEvidence>();
            graphEvidence[vertex.Key] = vertex.ToEvidence();

            try
            {
                vertex.Parent.UpdateValue(graphEvidence);
            }
            catch(InconsistentEvidenceException exception)
            {
                window.PopupControl.ShowText("Inconsistent evidence entered.");
                vertex.IsEvidenceEntered = false;
                vertex.Parent.UpdateBeliefs();
                vertex.Parent.UpdateValue();
            }
        }

        private void GraphControl_RetractButtonClicked(object sender, ValueEventArgs<BnVertexViewModel> e)
        {
            var vertex = e.Value;

            vertex.ClearEvidence();
            vertex.Parent.UpdateBeliefs();
            vertex.Parent.UpdateValue();
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