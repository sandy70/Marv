using LibPipeline;
using Microsoft.Surface.Presentation.Controls;
using System.Windows;
using System.Windows.Interactivity;

namespace Marv
{
    internal class YearSliderBehavior : Behavior<SurfaceSlider>
    {
        public void AssociatedObject_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue % 1 == 0)
            {
                MainWindow window = Application.Current.MainWindow as MainWindow;
                window.SelectedVertexValues = window.VertexValuesByYear[(int)e.NewValue];
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.ValueChanged += AssociatedObject_ValueChanged;
        }
    }
}