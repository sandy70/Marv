using System;
using System.Collections.ObjectModel;
using System.Windows;
using Marv.Common;
using Marv.Common.Graph;
using Marv.Controls.Graph;
using Telerik.Windows.Controls;

namespace Marv.Input
{
    public partial class MainWindow
    {
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

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Vertex = new Vertex
            {
                Key = "Vertex",
                Name = "My Vertex",
                IsExpanded = true,
                IsSelected = true,
                Units = "My Units",
                Description = "This is some random long description for this vertex. This will contain long sentences even running in paragraphs.",
                States = new ModelCollection<State>
                {
                    new State
                    {
                        Key = "State1",
                        Name = "State One"
                    },
                    new State
                    {
                        Key = "State2;lkajd;lkjfa;lkd",
                        Name = "State Two"
                    },
                    new State
                    {
                        Key = "State3",
                        Name = "State Three"
                    },
                }
            };

            this.Vertex.UpdateMostProbableState();
            this.NewNotificationButton.Click += NewNotificationButton_Click;
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