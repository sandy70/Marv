using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using Marv.Common;
using Marv.Common.Graph;
using Marv.Controls.Graph;
using Marv.Input.Properties;
using Microsoft.Win32;
using Telerik.Windows.Controls;

namespace Marv.Input
{
    public partial class MainWindow
    {
        public static readonly DependencyProperty GraphProperty =
            DependencyProperty.Register("Graph", typeof (Graph), typeof (MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedVertexProperty =
            DependencyProperty.Register("SelectedVertex", typeof (Vertex), typeof (MainWindow), new PropertyMetadata(null));

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

        public Vertex SelectedVertex
        {
            get
            {
                return (Vertex) GetValue(SelectedVertexProperty);
            }
            set
            {
                SetValue(SelectedVertexProperty, value);
            }
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Read the graph
            this.Graph = await Graph.ReadAsync(Settings.Default.FileName);
            this.Graph.SetValueToZero();

            foreach (var vertex in this.Graph.Vertices)
            {
                vertex.PropertyChanged += vertex_PropertyChanged;
            }

            this.OpenButton.Click += OpenButton_Click;
            this.SaveButton.Click += SaveButton_Click;
            this.VertexControl.CommandExecuted += VertexControl_CommandExecuted;
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Graph Evidence |*.graphevidence";
            dialog.Multiselect = false;

            if (dialog.ShowDialog() == true)
            {
                var graphEvidence = Utils.ReadJson<Dictionary<string, Evidence>>(dialog.FileName);
                this.Graph.SetEvidence(graphEvidence);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.Graph.GetEvidence().WriteJson("marv.graphevidence");
        }

        void vertex_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var vertex = sender as Vertex;

            if (vertex == null) return;

            if (e.PropertyName == "IsEvidenceEntered")
            {
                if (vertex.IsEvidenceEntered)
                {
                    if (!vertex.Commands.Contains(VertexCommands.Clear))
                    {
                        vertex.Commands.Add(VertexCommands.Clear);
                    }
                }
                else
                {
                    if (vertex.Commands.Contains(VertexCommands.Clear))
                    {
                        vertex.Commands.Remove(VertexCommands.Clear);
                    }
                }
            }
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
            this.SelectedVertex.SelectState(state);
        }

        private void VertexControl_CommandExecuted(object sender, Command<Vertex> command)
        {
            var vertexControl = sender as VertexControl;
            
            if (vertexControl == null) return;

            var vertex = vertexControl.Vertex;

            if (command == VertexCommands.Clear)
            {
                vertex.SetValue(0);
                vertex.IsEvidenceEntered = false;
                vertex.EvidenceString = null;
            }
        }
    }
}