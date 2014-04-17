using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using AutoMapper;
using Marv.Common;
using Marv.Common.Graph;
using Marv.Controls.Graph;
using Marv.Input.Properties;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Telerik.Windows.Controls;

namespace Marv.Input
{
    public partial class MainWindow
    {
        private Dictionary<string, string, string, double> modelEvidence = new Dictionary<string, string, string, double>();

        public static readonly DependencyProperty InputRowsProperty =
            DependencyProperty.Register("InputRows", typeof (ObservableCollection<dynamic>), typeof (MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty GraphProperty =
            DependencyProperty.Register("Graph", typeof (Graph), typeof (MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedVertexProperty =
            DependencyProperty.Register("SelectedVertex", typeof (Vertex), typeof (MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty NotificationsProperty =
            DependencyProperty.Register("Notifications", typeof (ObservableCollection<INotification>), typeof (MainWindow), new PropertyMetadata(new ObservableCollection<INotification>()));

        public MainWindow()
        {
            StyleManager.ApplicationTheme = new Windows8Theme();

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

        public ObservableCollection<dynamic> InputRows
        {
            get
            {
                return (ObservableCollection<dynamic>) GetValue(InputRowsProperty);
            }

            set
            {
                SetValue(InputRowsProperty, value);
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

        private void GraphControl_EvidenceEntered(object sender, Vertex e)
        {
            this.Graph.Run();

            var year = (string) this.InputGridView.CurrentCell.Column.Header;
            modelEvidence[year] = this.Graph.Evidence;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Read the graph
            this.Graph = await Graph.ReadAsync(Settings.Default.FileName);
            this.Graph.Value = null;

            this.OpenButton.Click += OpenButton_Click;
            this.SaveButton.Click += SaveButton_Click;

            this.GraphControl.EvidenceEntered -= GraphControl_EvidenceEntered;
            this.GraphControl.EvidenceEntered += GraphControl_EvidenceEntered;

            this.InputGridView.CurrentCellChanged -= InputGridView_CurrentCellChanged;
            this.InputGridView.CurrentCellChanged += InputGridView_CurrentCellChanged;

            this.VertexControl.CommandExecuted += VertexControl_CommandExecuted;
            this.VertexControl.EvidenceEntered += VertexControl_EvidenceEntered;

            var inputRows = new ObservableCollection<dynamic>();

            var row = new Dynamic();
            row["Section ID"] = "Section 1";
            row["2001"] = "";
            row["2002"] = "";

            inputRows.Add(row);

            this.InputRows = inputRows;
        }

        void InputGridView_CurrentCellChanged(object sender, GridViewCurrentCellChangedEventArgs e)
        {
            if (e.NewCell != null)
            {
                Console.WriteLine(e.NewCell.Column.Header);

                var year = (string)e.NewCell.Column.Header;

                if (!modelEvidence.ContainsKey(year))
                {
                    modelEvidence[year] = new Dictionary<string, string, double>();
                }

                this.Graph.Evidence = modelEvidence[year];
            }
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Graph Evidence |*.vertices",
                Multiselect = false
            };

            if (dialog.ShowDialog() == false) return;

            var list = Utils.ReadJson(dialog.FileName) as JArray;

            foreach (var item in list)
            {
                var result = JsonConvert.DeserializeAnonymousType(item.ToString(), new
                {
                    EvidenceString = "",
                    Key = "",
                    Evidence = new Dictionary<string, double>()
                });

                Mapper.DynamicMap(result, this.Graph.Vertices[result.Key]);
            }

            this.Graph.Run();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.Graph
                .Vertices
                .Where(vertex => vertex.IsEvidenceEntered)
                .Select(vertex => new
                {
                    vertex.EvidenceString,
                    vertex.Key,
                    vertex.Evidence
                })
                .WriteJson("marv.vertices");
        }

        private void VertexControl_CommandExecuted(object sender, Command<Vertex> command)
        {
            var vertexControl = sender as VertexControl;

            if (vertexControl == null) return;

            var vertex = vertexControl.Vertex;

            if (command == VertexCommands.Clear)
            {
                vertex.SetValue(0);
                vertex.EvidenceString = null;
            }
        }

        private void VertexControl_EvidenceEntered(object sender, Vertex e)
        {
            this.Graph.Run();
        }
    }
}