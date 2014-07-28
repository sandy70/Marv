using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Marv.Common;
using Marv.Common.Graph;
using Marv.Controls.Graph;
using Marv.Input.Properties;
using Microsoft.Win32;
using Newtonsoft.Json;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Telerik.Windows.Controls;
using ScatterPoint = OxyPlot.Series.ScatterPoint;

namespace Marv.Input
{
    public partial class MainWindow
    {
        public static readonly DependencyProperty DataPlotModelProperty =
            DependencyProperty.Register("DataPlotModel", typeof (PlotModel), typeof (MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty EndYearProperty =
            DependencyProperty.Register("EndYear", typeof (int), typeof (MainWindow), new PropertyMetadata(2000, ChangedEndYear));

        public static readonly DependencyProperty GraphProperty =
            DependencyProperty.Register("Graph", typeof (Graph), typeof (MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty InputRowsProperty =
            DependencyProperty.Register("InputRows", typeof (ObservableCollection<dynamic>), typeof (MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty IsInputToolbarEnabledProperty =
            DependencyProperty.Register("IsInputToolbarEnabled", typeof (bool), typeof (MainWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty NotificationsProperty =
            DependencyProperty.Register("Notifications", typeof (ObservableCollection<INotification>), typeof (MainWindow), new PropertyMetadata(new ObservableCollection<INotification>()));

        public static readonly DependencyProperty StartYearProperty =
            DependencyProperty.Register("StartYear", typeof (int), typeof (MainWindow), new PropertyMetadata(2000, ChangedStartYear));

        public LineEvidence LineEvidence;

        public PlotModel DataPlotModel
        {
            get
            {
                return (PlotModel) GetValue(DataPlotModelProperty);
            }
            set
            {
                SetValue(DataPlotModelProperty, value);
            }
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

        public bool IsInputToolbarEnabled
        {
            get
            {
                return (bool) GetValue(IsInputToolbarEnabledProperty);
            }
            set
            {
                SetValue(IsInputToolbarEnabledProperty, value);
            }
        }

        public bool IsYearPlot { get; set; }

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

        private void AddPlotInfo(string title, string xAxis)
        {
            this.DataPlotModel.Title = title;

            this.DataPlotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = xAxis
            });

            this.DataPlotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Input Data"
            });
        }

        private void AddPointsToPlot(object entry, ScatterSeries series1, CandleStickSeries series2, double index)
        {
            if ((!entry.ToString().Contains(":")))
            {
                var value = Convert.ToDouble(entry);
                series1.Points.Add(new ScatterPoint(index, value));
            }
            else if (entry.ToString().Split(":".ToArray()).Length == 2)
            {
                var valueSet = entry.ToString().Split(":".ToArray());
                series2.Items.Add(new HighLowItem(index, Convert.ToDouble(valueSet[0]), Convert.ToDouble(valueSet[1]),
                    Convert.ToDouble(valueSet[0]), Convert.ToDouble(valueSet[1])));
            }
        }

        private void AddSectionButton_Click(object sender, RoutedEventArgs e)
        {
            var row = new Dynamic();
            var sectionId = "Section " + (this.InputRows.Count + 1);
            row[CellModel.SectionIdHeader] = sectionId;
            this.LineEvidence.SectionEvidences.Add(new SectionEvidence {Id = sectionId});

            foreach (var year in this.LineEvidence.Years)
            {
                row[year.ToString()] = "";
            }

            this.InputRows.Add(row);
        }

        private void ClearAllButton_Click(object sender, RoutedEventArgs e)
        {
            this.InputGridView.SelectAll();

            foreach (var cell in this.InputGridView.SelectedCells)
            {
                if (this.Graph.SelectedVertex != null)
                {
                    this.Graph.SelectedVertex.EvidenceString = null;
                    this.Graph.SelectedVertex.UpdateEvidence();
                }

                var row = cell.Item as Dynamic;
                var sectionId = row[CellModel.SectionIdHeader] as string;
                var year = (string) cell.Column.Header;
                if (year != CellModel.SectionIdHeader)
                {
                    row[year] = null;
                    var evidence = this.Graph.SelectedVertex.GetData();
                    row[year] = evidence;

                    this.LineEvidence
                        .SectionEvidences[sectionId]
                        .YearEvidences[Convert.ToInt32(year)]
                        .GraphEvidence[this.Graph.SelectedVertex.Key] = evidence;
                }
            }

            this.InputGridView.UnselectAll();
        }

        private void CopyAcrossColumns_Click(object sender, RoutedEventArgs e)
        {
            if (this.InputGridView.SelectedCells.Count == 1)
            {
                var selectedCellModel = this.InputGridView.SelectedCells[0].ToModel();

                if (selectedCellModel.IsColumnSectionId)
                {
                    return;
                }

                var vertexEvidence = selectedCellModel.Data as VertexEvidence;

                foreach (var column in this.InputGridView.Columns)
                {
                    var cellModel = new CellModel(selectedCellModel.Row, column.Header as string);

                    if (cellModel.IsColumnSectionId) continue;

                    this.SetCell(cellModel, vertexEvidence.String);
                }
            }
        }

        private void CopyAcrossRows_Click(object sender, RoutedEventArgs e)
        {
            if (this.InputGridView.SelectedCells.Count == 1)
            {
                var selectedCellModel = this.InputGridView.SelectedCells[0].ToModel();

                if (selectedCellModel.IsColumnSectionId)
                {
                    return;
                }

                var selectedCellData = selectedCellModel.Data as VertexEvidence;

                foreach (var row in this.InputRows)
                {
                    var cellModel = new CellModel(row, selectedCellModel.Header);
                    this.SetCell(cellModel, selectedCellData.String);
                }
            }
        }

        private void CreateInputButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Graph == null)
            {
                this.Notifications.Push(new NotificationIndeterminate
                {
                    Name = "No network available!",
                    Description = "You cannot create and input if no network is opened."
                });

                return;
            }

            var inputRows = new ObservableCollection<dynamic>();
            var row = new Dynamic();

            var sectionId = "Section 1";
            row[CellModel.SectionIdHeader] = sectionId;

            this.LineEvidence = new LineEvidence
            {
                GraphGuid = this.Graph.Guid
            };

            this.LineEvidence.SectionEvidences.Add(new SectionEvidence {Id = sectionId});

            for (var year = this.StartYear; year <= this.EndYear; year++)
            {
                this.LineEvidence.SectionEvidences[sectionId].YearEvidences.Add(new YearEvidence {Year = year});
                row[year.ToString()] = "";
            }

            inputRows.Add(row);

            this.InputRows = inputRows;
            this.IsInputToolbarEnabled = true;
            this.Graph.Belief = null;
            this.Graph.SetEvidence(null);

            foreach (var column in this.InputGridView.Columns)
            {
                column.Width = 70;
            }
        }

        private void GraphControl_EvidenceEntered(object sender, Vertex vertex)
        {
            this.Graph.Run();

            if (this.InputGridView.CurrentCell == null) return;

            var vertexData = vertex.GetData();

            var cellModel = this.InputGridView.CurrentCell.ToModel();

            if (cellModel.IsColumnSectionId) return;

            cellModel.Data = vertexData;

            this.LineEvidence
                .SectionEvidences[cellModel.SectionId]
                .YearEvidences[cellModel.Year]
                .GraphEvidence[this.Graph.SelectedVertex.Key] = vertexData;
        }

        private void GraphControl_GraphChanged(object sender, ValueChangedArgs<Graph> e)
        {
            this.LineEvidence = new LineEvidence();
            this.UpdateGrid();
        }

        private void GraphControl_SelectionChanged(object sender, Vertex e)
        {
            this.UpdateGrid();
        }

        private void InitializePlot()
        {
            if (this.InputGridView.SelectedCells.Count == 1)
            {
                var series1 = new ScatterSeries();
                series1.MarkerFill = OxyColors.Green;
                var series2 = new CandleStickSeries();
                series2.Color = OxyColors.Green;
                this.DataPlotModel = new PlotModel();

                var model = new CellModel(this.InputGridView.SelectedCells[0]);
                if (model.IsColumnSectionId)
                {
                    return;
                }
                if (IsYearPlot)
                {
                    AddPlotInfo("Input Data for the Year " + model.Year, "Section ID");
                    foreach (var row in this.InputRows)
                    {
                        var rowIndex = this.InputRows.IndexOf(row);
                        var entry = row[model.Header];
                        AddPointsToPlot(entry, series1, series2, Convert.ToDouble(rowIndex));
                    }
                }
                else
                {
                    AddPlotInfo("Input Data for " + model.SectionId, "Year");
                    var row = model.Row;
                    foreach (var column in this.InputGridView.Columns)
                    {
                        var year = column.Header.ToString();
                        var entry = row[year];
                        if (year != CellModel.SectionIdHeader && entry != "")
                        {
                            AddPointsToPlot(entry, series1, series2, Convert.ToDouble(year));
                        }
                    }
                }
                this.DataPlotModel.Series.Add(series1);
                this.DataPlotModel.Series.Add(series2);
                this.DataPlotModel.InvalidatePlot(true);
            }
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Read the graph
            this.Graph = await Graph.ReadAsync(Settings.Default.FileName);
            this.Graph.Belief = null;
            IsYearPlot = true;

            this.AddSectionButton.Click += AddSectionButton_Click;
            this.ClearAllButton.Click += this.ClearAllButton_Click;
            this.CreateInputButton.Click += CreateInputButton_Click;
            this.OpenButton.Click += OpenButton_Click;
            this.SaveButton.Click += SaveButton_Click;
            this.PlotButton.Click += PlotButton_Click;
            this.CopyAcrossColumns.Click += CopyAcrossColumns_Click;
            this.CopyAcrossRows.Click += CopyAcrossRows_Click;

            this.TypePlotButtonYear.Checked += TypePlotButtonYear_Checked;
            this.TypePlotButtonSection.Checked += TypePlotButtonSection_Checked;

            this.GraphControl.EvidenceEntered += GraphControl_EvidenceEntered;
            this.GraphControl.GraphChanged += GraphControl_GraphChanged;
            this.GraphControl.SelectionChanged += GraphControl_SelectionChanged;

            this.InputGridView.AutoGeneratingColumn += InputGridView_AutoGeneratingColumn;
            this.InputGridView.CellEditEnded += InputGridView_CellEditEnded;
            this.InputGridView.CellValidating += InputGridView_CellValidating;

            this.InputGridView.Pasted += InputGridView_Pasted;
            this.InputGridView.PastingCellClipboardContent += InputGridView_PastingCellClipboardContent;

            this.InputGridView.KeyDown += InputGridView_KeyDown;
            this.InputGridView.CurrentCellChanged += InputGridView_CurrentCellChanged;

            this.VertexControl.CommandExecuted += this.VertexControl_CommandExecuted;
            this.VertexControl.EvidenceEntered += this.VertexControl_EvidenceEntered;
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Input|*.input",
                Multiselect = false
            };

            if (dialog.ShowDialog() == false) return;

            this.LineEvidence = Utils.ReadJson<LineEvidence>(dialog.FileName);

            var isCorrectInput = true;

            if (this.LineEvidence.GraphGuid != this.Graph.Guid)
            {
                RadWindow.Confirm("This input was not created for the loaded network. Do you still want to open it?",
                    (o1, e1) => isCorrectInput = e1.DialogResult.Value);
            }

            if (!isCorrectInput) return;

            var inputRows = new ObservableCollection<dynamic>();

            foreach (var sectionEvidence in this.LineEvidence.SectionEvidences)
            {
                var row = new Dynamic();
                row[CellModel.SectionIdHeader] = sectionEvidence.Id;

                foreach (var yearEvidence in sectionEvidence.YearEvidences)
                {
                    if (yearEvidence.GraphEvidence.ContainsKey(this.Graph.SelectedVertex.Key))
                    {
                        var input = yearEvidence.GraphEvidence[this.Graph.SelectedVertex.Key];
                        row[yearEvidence.Year.ToString()] = input;
                    }
                    else
                    {
                        row[yearEvidence.Year.ToString()] = "";
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
            this.IsInputToolbarEnabled = true;
            this.Graph.Run();
        }

        private void PlotButton_Click(object sender, RoutedEventArgs e)
        {
            InitializePlot();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Input|*.input",
            };

            if (dialog.ShowDialog() == false) return;

            if (dialog.FileName != null)
            {
                // User Formatting.None to save space. These files are not intended to be human readable.
                this.LineEvidence.WriteJson(dialog.FileName, Formatting.None);
            }
        }

        private void TypePlotButtonSection_Checked(object sender, RoutedEventArgs e)
        {
            this.IsYearPlot = false;
        }

        private void TypePlotButtonYear_Checked(object sender, RoutedEventArgs e)
        {
            this.IsYearPlot = true;
        }

        private void UpdateGrid()
        {
            if (this.InputRows == null || this.Graph.SelectedVertex == null) return;

            foreach (var row in this.InputRows)
            {
                foreach (var column in this.InputGridView.Columns)
                {
                    var cellModel = new CellModel(row, column.Header as string);

                    if (cellModel.IsColumnSectionId) continue;

                    var sectionEvidence = this.LineEvidence.SectionEvidences[cellModel.SectionId];

                    if (!sectionEvidence.YearEvidences.ContainsKey(cellModel.Year))
                    {
                        sectionEvidence.YearEvidences.Add(new YearEvidence {Year = cellModel.Year});
                    }

                    var graphEvidence = sectionEvidence.YearEvidences[cellModel.Year].GraphEvidence;

                    if (graphEvidence.ContainsKey(this.Graph.SelectedVertex.Key))
                    {
                        cellModel.Data = graphEvidence[this.Graph.SelectedVertex.Key];
                    }
                    else
                    {
                        cellModel.Data = "";
                    }
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

        private void VertexControl_EvidenceEntered(object sender, Vertex vertex)
        {
            this.Graph.Run();

            if (this.InputGridView.CurrentCell == null) return;

            var cellModel = this.InputGridView.CurrentCell.ToModel();
            var vertexData = vertex.GetData();

            cellModel.Data = vertexData;
            this.LineEvidence
                .SectionEvidences[cellModel.SectionId]
                .YearEvidences[cellModel.Year]
                .GraphEvidence[this.Graph.SelectedVertex.Key] = vertexData;
        }
    }
}