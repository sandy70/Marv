using LibBn;
using System;
using System.Windows;
using System.Windows.Interactivity;
using GroupViewer.Properties;

namespace GroupViewer
{
    internal class MainWindowBehavior : Behavior<MainWindow>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Closed += AssociatedObject_Closed;
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Closed(object sender, EventArgs e)
        {
            if (Settings.Default.IsGraphSaveEnabled)
            {
                BnGraphWriter.WritePositions(Properties.Settings.Default.FileName, this.AssociatedObject.GraphControl.SourceGraph);
            }

            Properties.Settings.Default.Save();
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            this.AssociatedObject.GraphControl.FileNotFound += GraphControl_FileNotFound;
        }

        private void GraphControl_FileNotFound(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("File not found!!");
        }
    }
}