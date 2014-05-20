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
        public static readonly DependencyProperty StartYearProperty =
            DependencyProperty.Register("StartYear", typeof (int), typeof (MainWindow), new PropertyMetadata(2000, ChangedStartYear));

        public static readonly DependencyProperty EndYearProperty =
            DependencyProperty.Register("EndYear", typeof (int), typeof (MainWindow), new PropertyMetadata(2000, ChangedEndYear));

        public static readonly DependencyProperty InputRowsProperty =
            DependencyProperty.Register("InputRows", typeof (ObservableCollection<dynamic>), typeof (MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty GraphProperty =
            DependencyProperty.Register("Graph", typeof (Graph), typeof (MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedVertexProperty =
            DependencyProperty.Register("SelectedVertex", typeof (Vertex), typeof (MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty NotificationsProperty =
            DependencyProperty.Register("Notifications", typeof (ObservableCollection<INotification>), typeof (MainWindow), new PropertyMetadata(new ObservableCollection<INotification>()));

        private Dictionary<int, string, string, double> ModelEvidence = new Dictionary<int, string, string, double>();

        public MainWindow()
        {
            StyleManager.ApplicationTheme = new Windows8Theme();

            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        public int EndYear
        {
            get
            {
                return (int) GetValue(EndYearProperty);
            }

            set
            {
                SetValue(EndYearProperty, value);
            }
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

        public int StartYear
        {
            get
            {
                return (int) GetValue(StartYearProperty);
            }

            set
            {
                SetValue(StartYearProperty, value);
            }
        }

        private static void ChangedEndYear(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var mainWindow = d as MainWindow;

            if (mainWindow == null) return;

            if (mainWindow.EndYear < mainWindow.StartYear)
            {
                mainWindow.StartYear = mainWindow.EndYear;
            }
        }

        private static void ChangedStartYear(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var mainWindow = d as MainWindow;

            if (mainWindow == null) return;

            if (mainWindow.StartYear > mainWindow.EndYear)
            {
                mainWindow.EndYear = mainWindow.StartYear;
            }
        }

        private void CreateInputButton_Click(object sender, RoutedEventArgs e)
        {
            var inputRows = new ObservableCollection<dynamic>();
            var row = new Dynamic();
            row["Section ID"] = "Section 1";

            for (var year = this.StartYear; year <= this.EndYear; year++)
            {
                row[year.ToString()] = "";
            }

            inputRows.Add(row);
            this.InputRows = inputRows;
            this.ModelEvidence = new Dictionary<int, string, string, double>();
            this.Graph.Belief = null;
            this.Graph.Evidence = null;
        }

        private void GraphControl_EvidenceEntered(object sender, Vertex e)
        {
            this.UpdateModelEvidence();
        }

        private void InputGridView_CurrentCellChanged(object sender, GridViewCurrentCellChangedEventArgs e)
        {
            if (e.NewCell != null)
            {
                this.GraphControl.IsEnabled = true;
                this.VertexControl.IsEnabled = true;

                try
                {
                    var year = Convert.ToInt32((string) e.NewCell.Column.Header);

                    if (this.ModelEvidence.ContainsKey(year))
                    {
                        this.Graph.Evidence = this.ModelEvidence[year];
                    }
                    else
                    {
                        this.Graph.Evidence = null;
                    }
                }
                catch (FormatException)
                {
                }
            }
            else
            {
                this.GraphControl.IsEnabled = true;
                this.VertexControl.IsEnabled = true;
            }
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Read the graph
            this.Graph = await Graph.ReadAsync(Settings.Default.FileName);
            this.Graph.Value = null;

            this.OpenButton.Click += OpenButton_Click;
            this.SaveButton.Click += SaveButton_Click;

            this.CreateInputButton.Click -= CreateInputButton_Click;
            this.CreateInputButton.Click += CreateInputButton_Click;

            this.GraphControl.EvidenceEntered -= GraphControl_EvidenceEntered;
            this.GraphControl.EvidenceEntered += GraphControl_EvidenceEntered;

            this.InputGridView.CurrentCellChanged -= InputGridView_CurrentCellChanged;
            this.InputGridView.CurrentCellChanged += InputGridView_CurrentCellChanged;

            this.VertexControl.CommandExecuted += VertexControl_CommandExecuted;
            this.VertexControl.EvidenceEntered += VertexControl_EvidenceEntered;
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Input|*.input",
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
            this.ModelEvidence.WriteJson("marv.input");
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
            this.UpdateModelEvidence();
        }

        private void UpdateModelEvidence()
        {
            if (this.InputGridView.CurrentCell == null)
            {
                this.Notifications.Push(new NotificationTimed
                {
                    Description = "You must select a year before you can enter evidence.",
                    Name =  "Select Year!"
                });
            }
            else
            {
                this.Graph.Run();

                var year = Convert.ToInt32((string)this.InputGridView.CurrentCell.Column.Header);

                this.ModelEvidence[year] = this.Graph.Evidence;
            }
        }
    }
}