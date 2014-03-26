using Marv.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using Marv.Common;
using Marv.Common.Graph;

namespace Marv.Input
{
    internal class SliderProgressBarBehaviorNew : Behavior<SliderProgressBar>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.MouseDoubleClick += this.AssociatedObject_MouseDoubleClick;
            // this.AssociatedObject.ValueEntered += this.AssociatedObject_ValueEntered;
        }

        private void AssociatedObject_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var window = this.AssociatedObject.FindParent<MainWindow>();
            window.SelectState(this.AssociatedObject.DataContext as State);
        }

        //private void AssociatedObject_ValueEntered(object sender, ValueEventArgs<double> e)
        //{
        //    var statesControl = this.AssociatedObject.FindParent<StatesControl>();
        //    var selectedState = this.AssociatedObject.DataContext as State;

        //    statesControl.RaiseEvent(new ValueEventArgs<State>
        //    {
        //        RoutedEvent = StatesControl.ValueEnteredEvent,
        //        Value = selectedState
        //    });
        //}
    }
}
