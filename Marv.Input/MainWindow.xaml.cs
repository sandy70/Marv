﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Marv.Common;
using Marv.Common.Graph;
using Marv.Controls.Graph;
using Marv.Input.Properties;
using Microsoft.Win32;
using Telerik.Windows;
using Telerik.Windows.Controls;

namespace Marv.Input
{
    public partial class MainWindow
    {
        public static readonly DependencyProperty EndYearProperty =
            DependencyProperty.Register("EndYear", typeof (int), typeof (MainWindow), new PropertyMetadata(2000, ChangedEndYear));

        public static readonly DependencyProperty GraphProperty =
            DependencyProperty.Register("Graph", typeof (Graph), typeof (MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty InputRowsProperty =
            DependencyProperty.Register("InputRows", typeof (ObservableCollection<dynamic>), typeof (MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty NotificationsProperty =
            DependencyProperty.Register("Notifications", typeof (ObservableCollection<INotification>), typeof (MainWindow), new PropertyMetadata(new ObservableCollection<INotification>()));

        public static readonly DependencyProperty SelectedVertexProperty =
            DependencyProperty.Register("SelectedVertex", typeof (Vertex), typeof (MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty StartYearProperty =
            DependencyProperty.Register("StartYear", typeof (int), typeof (MainWindow), new PropertyMetadata(2000, ChangedStartYear));

        // Dictionary<sectionID, year, vertexKey, vertexEvidence>
        public Dictionary<string, int, string, VertexEvidence> LineEvidence = new Dictionary<string, int, string, VertexEvidence>();

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

        public MainWindow()
        {
            StyleManager.ApplicationTheme = new Windows8Theme();

            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        private void AddSectionButton_Click(object sender, RoutedEventArgs e)
        {
            var row = new Dynamic();
            try
            {
                row["Section ID"] = "Section " + (this.InputRows.Count + 1);
            }
            catch (NullReferenceException)
            {
                return;
            }

            for (var year = this.StartYear; year <= this.EndYear; year++)
            {
                row[year.ToString()] = "";
            }

            this.InputRows.Add(row);
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
            this.LineEvidence = new Dictionary<string, int, string, VertexEvidence>();
            this.Graph.Belief = null;
            this.Graph.SetEvidence(null);
            foreach (var column in this.InputGridView.Columns)
            {
                column.Width = 70;
            }
        }

        private void GraphControl_EvidenceEntered(object sender, Vertex e)
        {
            this.UpdateModelEvidence();
        }

        private void GraphControl_SelectionChanged(object sender, Vertex e)
        {
            this.UpdateGrid();
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Read the graph
            this.Graph = await Graph.ReadAsync(Settings.Default.FileName);
            this.Graph.FileName = Settings.Default.FileName;
            this.Graph.Belief = null;

            this.AddSectionButton.Click += AddSectionButton_Click;
            this.OpenButton.Click += OpenButton_Click;
            this.SaveButton.Click += SaveButton_Click;
            this.ClearAll.Click += ClearAll_Click;

            this.CreateInputButton.Click -= CreateInputButton_Click;
            this.CreateInputButton.Click += CreateInputButton_Click;

            this.GraphControl.EvidenceEntered += GraphControl_EvidenceEntered;
            this.GraphControl.SelectionChanged += GraphControl_SelectionChanged;

            this.InputGridView.AutoGeneratingColumn += InputGridView_AutoGeneratingColumn;
            this.InputGridView.CellEditEnded += InputGridView_CellEditEnded;

            this.InputGridView.Pasted += InputGridView_Pasted;
            this.InputGridView.PastingCellClipboardContent += InputGridView_PastingCellClipboardContent;

            this.InputGridView.KeyDown += InputGridView_KeyDown;
            this.InputGridView.CurrentCellChanged += InputGridView_CurrentCellChanged;

            this.VertexControl.CommandExecuted += VertexControl_CommandExecuted;
            this.VertexControl.EvidenceEntered += VertexControl_EvidenceEntered;
        }


           

        private int GetLinesFromClipboard(String text)
        {
            return text.Split('\n').Length - 1;
        }

        private void InputGridView_Pasting(object sender, GridViewClipboardEventArgs e)
        {
            if ((GetLinesFromClipboard(Clipboard.GetText()) > this.InputRows.Count))
            {
                e.Cancel = true;
            }
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Input|*.input",
                Multiselect = false
            };

            if (dialog.ShowDialog() == false) return;

            this.LineEvidence = Utils.ReadJson<Dictionary<string, int, string, VertexEvidence>>(dialog.FileName);

            var inputRows = new ObservableCollection<dynamic>();

            foreach (var section in this.LineEvidence.Keys)
            {
                var row = new Dynamic();
                row["Section ID"] = section;

                var sectionEvidence = this.LineEvidence[section];
                
                foreach (var year in sectionEvidence.Keys)
                {
                    var yearEvidence = this.LineEvidence[section][year];

                    if (yearEvidence.ContainsKey(this.SelectedVertex.Key))
                    {
                        var input = yearEvidence[this.SelectedVertex.Key];
                        row[year.ToString()] = input;
                    }
                    else
                    {
                        row[year.ToString()] = "";
                    }                  
                }
                inputRows.Add(row);
              
            }

            this.InputRows = inputRows;

            var item = this.InputGridView.Items[0];
            var column = this.InputGridView.Columns[1];
            var cellToEdit = new GridViewCellInfo(item, column, this.InputGridView);

            this.InputGridView.IsSynchronizedWithCurrentItem = true;
            this.InputGridView.SelectedItem = item;
            this.InputGridView.CurrentItem = item;
            this.InputGridView.CurrentCellInfo = cellToEdit;

            //this.Graph.SetNetworkEvidence(this.ModelEvidence.First().Value);
            this.Graph.Run();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog 
            {
                Filter = "Input|*.input",
            };

            if (dialog.ShowDialog() == false) return;
            if (dialog.FileName != null)
            {
                this.LineEvidence.WriteJson(dialog.FileName);
            }
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            this.InputGridView.SelectAll();
            foreach (var cell in this.InputGridView.SelectedCells)
            {
                this.SelectedVertex.EvidenceString = null;
                this.SelectedVertex.UpdateEvidence();

                var row = cell.Item as Dynamic;
                var sectionId = row["Section ID"] as string;
                var year = (string)cell.Column.Header;
                if (year != "Section ID")
                {
                    row[year] = null;
                    var evidence = new VertexEvidence(this.SelectedVertex.Evidence, this.SelectedVertex.EvidenceString);
                    row[year] = evidence;
                    this.LineEvidence[sectionId, Convert.ToInt32(year), this.SelectedVertex.Key] = evidence;

                }
            }
            this.InputGridView.UnselectAll();
        }

        private void UpdateGrid()
        {
            foreach (var row in this.InputRows)
            {
                foreach (var year in Enumerable.Range(this.StartYear, this.EndYear - this.StartYear + 1))
                {
                    var sectionId = row["Section ID"] as string;

                    if (sectionId != null)
                    {
                        try
                        {
                            var evidence = this.LineEvidence[sectionId][year][this.SelectedVertex.Key];
                            row[year.ToString()] = evidence;
                        }
                        catch (KeyNotFoundException)
                        {
                            row[year.ToString()] = null;
                        }
                    }
                }
            }
        }

        private void UpdateModelEvidence()
        {
            if (this.InputGridView.CurrentCell == null)
            {
                this.Notifications.Push(new NotificationTimed
                {
                    Description = "You must select a year before you can enter evidence.",
                    Name = "Select Year!"
                });
            }
            else
            {
                this.Graph.Run();

                var year = Convert.ToInt32((string) this.InputGridView.CurrentCell.Column.Header);
                var row = this.InputGridView.CurrentCell.ParentRow.DataContext as Dynamic;
                if (row != null)
                {
                    var sectionId = row["Section ID"] as string;
                    this.LineEvidence[sectionId, year] = this.Graph.GetEvidence();
                }
            }
        }

        private void VertexControl_CommandExecuted(object sender, Command<Vertex> command)
        {
            var vertexControl = sender as VertexControl;

            if (vertexControl == null) return;

            var vertex = vertexControl.Vertex;

            if (command == VertexCommands.Clear)
            {
                vertex.Evidence = null;
                vertex.EvidenceString = null;
            }
        }

        private void VertexControl_EvidenceEntered(object sender, Vertex e)
        {
            this.UpdateModelEvidence();
            this.UpdateGrid();
        }
    }
}