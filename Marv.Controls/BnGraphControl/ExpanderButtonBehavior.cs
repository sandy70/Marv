﻿using LibNetwork;
using System.Windows;
using System.Windows.Interactivity;
using Telerik.Windows.Controls;

namespace Marv.Controls
{
    internal class ExpanderButtonBehavior : Behavior<RadButton>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Click += AssociatedObject_Click;
        }

        private void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            var vertexViewModel = this.AssociatedObject.DataContext as VertexViewModel;
            vertexViewModel.IsExpanded = !vertexViewModel.IsExpanded;
        }
    }
}