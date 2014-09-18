using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Telerik.Windows.Controls;

namespace Marv.Input
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        public static readonly DependencyProperty GraphProperty =
            DependencyProperty.Register("Graph", typeof (Graph), typeof (MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty NotificationsProperty =
            DependencyProperty.Register("Notifications", typeof (NotificationCollection), typeof (MainWindow), new PropertyMetadata(new NotificationCollection()));

        public static readonly DependencyProperty SelectedSectionIdProperty =
            DependencyProperty.Register("SelectedSectionId", typeof (string), typeof (MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedYearProperty =
            DependencyProperty.Register("SelectedYear", typeof (int), typeof (MainWindow), new PropertyMetadata(int.MinValue));

        private LineData lineData;

        public Graph Graph
        {
            get
            {
                return (Graph) GetValue(GraphProperty);
            }

            set
            {
                SetValue(GraphProperty, value);
            }
        }

        public LineData LineData
        {
            get
            {
                return this.lineData;
            }

            set
            {
                if (value.Equals(this.lineData))
                {
                    return;
                }

                this.lineData = value;
                this.RaisePropertyChanged();
            }
        }

        public NotificationCollection Notifications
        {
            get
            {
                return (NotificationCollection) GetValue(NotificationsProperty);
            }

            set
            {
                SetValue(NotificationsProperty, value);
            }
        }

        public string SelectedSectionId
        {
            get
            {
                return (string) GetValue(SelectedSectionIdProperty);
            }
            set
            {
                SetValue(SelectedSectionIdProperty, value);
            }
        }

        public int SelectedYear
        {
            get
            {
                return (int) GetValue(SelectedYearProperty);
            }
            set
            {
                SetValue(SelectedYearProperty, value);
            }
        }

        public MainWindow()
        {
            StyleManager.ApplicationTheme = new Windows8Theme();
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null && propertyName != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void GraphControl_EvidenceEntered(object sender, Vertex vertex)
        {
            this.LineDataControl.SetSelectedCells(vertex.Data);
        }

        private void GraphControl_GraphChanged(object sender, ValueChangedArgs<Graph> e)
        {
            this.LineData = new LineData();
            this.LineData.Sections["Section 1"] = new Dict<int, string, VertexData>();
        }

        private void LineDataControl_NotificationIssued(object sender, Notification notification)
        {
            this.Notifications.Add(notification);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.GraphControl.EvidenceEntered += GraphControl_EvidenceEntered;
            this.GraphControl.GraphChanged += GraphControl_GraphChanged;

            this.LineDataControl.NotificationIssued -= LineDataControl_NotificationIssued;
            this.LineDataControl.NotificationIssued += LineDataControl_NotificationIssued;

            this.VertexControl.EvidenceEntered += this.GraphControl_EvidenceEntered;
        }
    }
}