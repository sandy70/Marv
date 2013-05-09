using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Marv
{
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty LocationsProperty =
        DependencyProperty.Register("Locations", typeof(IEnumerable<ILocation>), typeof(MainWindow), new PropertyMetadata(null));

        public IEnumerable<ILocation> Locations
        {
            get { return (IEnumerable<ILocation>)GetValue(LocationsProperty); }
            set { SetValue(LocationsProperty, value); }
        }

        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
