using LibPipeline;
using Microsoft.Win32;
using System;
using System.Windows.Interactivity;

namespace MapViewer
{
    internal class MainWindowBehavior : Behavior<MainWindow>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.AssociatedObject.Locations = MultiLocationReader.ReadExcel(@"D:\Data\BP\BpData.xlsx");

            foreach (var location in this.AssociatedObject.Locations)
            {
                Console.WriteLine(location.Latitude + ", " + location.Longitude);
            }
        }
    }
}