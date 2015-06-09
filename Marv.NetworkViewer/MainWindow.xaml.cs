using System.IO;
using System.Windows;
using Marv.NetworkViewer.Properties;
using Telerik.Windows.Controls;

namespace Marv.NetworkViewer
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            StyleManager.ApplicationTheme = new Windows8Theme();
            InitializeComponent();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}