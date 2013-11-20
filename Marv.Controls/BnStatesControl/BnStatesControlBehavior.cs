﻿using Marv.Common;
using System.Windows.Interactivity;

namespace Marv.Controls
{
    internal class BnStatesControlBehavior : Behavior<BnStatesControl>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.StateDoubleClicked += AssociatedObject_StateDoubleClicked;
            this.AssociatedObject.ValueEntered += AssociatedObject_ValueEntered;
        }

        private void AssociatedObject_StateDoubleClicked(object sender, ValueEventArgs<State> e)
        {
            var parentGraphControl = this.AssociatedObject.FindParent<GraphControl>();
            var state = e.Value;
            var vertex = this.AssociatedObject.DataContext as Vertex;

            parentGraphControl.RaiseEvent(new GraphControlEventArgs
            {
                RoutedEvent = GraphControl.StateDoubleClickedEvent,
                State = state,
                Vertex = vertex
            });
        }

        private void AssociatedObject_ValueEntered(object sender, ValueEventArgs<State> e)
        {
            var vertexViewModel = this.AssociatedObject.DataContext as Vertex;
            vertexViewModel.IsEvidenceEntered = true;
        }
    }
}