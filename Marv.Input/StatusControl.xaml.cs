using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
using Marv.Common; 
using Marv.Input;

namespace Marv.Input
{
    /// <summary>
    /// Interaction logic for StatusControl.xaml
    /// </summary>
    public partial class StatusControl
    {
        public static readonly DependencyProperty BarVisibilityProperty =
            DependencyProperty.Register("BarVisibility", typeof (Visibility), typeof (StatusControl), new PropertyMetadata(Visibility.Hidden));

        public Visibility BarVisibility
        {
            get { return (Visibility)GetValue(BarVisibilityProperty); }
            set { SetValue(BarVisibilityProperty, value); }
        }

        private ObservableCollection<Notification> Notifications;
        public StatusControl()
        {
            InitializeComponent();
            Notifications.CollectionChanged += (o, e) =>
            {
                if (Notifications.Count == 0)
                {
                    PercentBlock.Text = "";
                    BarVisibility = Visibility.Hidden;
                    LoadingBlock.Text = "";
                }
                else
                {
                    PercentBlock.Text = Math.Abs(Notifications[Notifications.Count - 1].Value - 100).ToString(CultureInfo.CurrentCulture) + " %";
                    LoadingBlock.Text = Notifications[Notifications.Count - 1].Description + " Loading...";
                    BarVisibility = Visibility.Visible;
                }
            };
        }

        
    }
}
