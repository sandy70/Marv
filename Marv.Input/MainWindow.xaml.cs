using Telerik.Windows.Controls;

namespace Marv.Input
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            StyleManager.ApplicationTheme = new Windows8TouchTheme();

            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // var data = new Dictionary<
        }
    }
}