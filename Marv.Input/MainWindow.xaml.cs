using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using Marv.Common;
using Marv.Common.Graph;
using Marv.Controls.Graph;
using Marv.Input.Properties;
using Telerik.Windows.Controls;

namespace Marv.Input
{
    public partial class MainWindow
    {
        public static readonly DependencyProperty GraphProperty =
            DependencyProperty.Register("Graph", typeof (Graph), typeof (MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty VertexProperty =
            DependencyProperty.Register("Vertex", typeof (Vertex), typeof (MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty NotificationsProperty =
            DependencyProperty.Register("Notifications", typeof (ObservableCollection<INotification>), typeof (MainWindow), new PropertyMetadata(new ObservableCollection<INotification>()));

        public MainWindow()
        {
            StyleManager.ApplicationTheme = new Windows8TouchTheme();

            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

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

        public ObservableCollection<INotification> Notifications
        {
            get
            {
                return (ObservableCollection<INotification>) GetValue(NotificationsProperty);
            }
            set
            {
                SetValue(NotificationsProperty, value);
            }
        }

        public Vertex Vertex
        {
            get
            {
                return (Vertex) GetValue(VertexProperty);
            }
            set
            {
                SetValue(VertexProperty, value);
            }
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Read the graph
            this.Graph = await Graph.ReadAsync(Settings.Default.FileName);
        }

        private void NewNotificationButton_Click(object sender, RoutedEventArgs e)
        {
            this.Notifications.Push(new NotificationIndeterminate
            {
                Name = "Be Notified!",
                Description = this.Notifications.Count + ". Are you notified yet?",
            });
        }

        internal void SelectState(State state)
        {
            this.Vertex.SelectState(state);
        }

        private void VertexControl_CommandExecuted(object sender, Command<Vertex> command)
        {
            if (command == VertexCommands.Lock)
            {
                var vertexControl = sender as VertexControl;

                if (vertexControl != null)
                {
                    var vertex = vertexControl.Vertex;

                    if (vertex.IsLocked)
                    {
                        Console.WriteLine("Locked");
                    }
                    else
                    {
                        Console.WriteLine("UnLocked");
                    }
                }
            }
        }
    }
}