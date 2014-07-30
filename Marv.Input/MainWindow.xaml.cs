﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Marv.Common;
using Marv.Common.Graph;
using Microsoft.CSharp.RuntimeBinder;
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
        public enum LineType
        {
            Mean,
            Min,
            Max
        };

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

        private readonly NotificationTimed notificationBadCurrentCell = new NotificationTimed
        {
            Name = "Warning!",
            Description = "No section/year selected for input.",
            IsMuteable = true
        };

        public bool IsYearPlot = true;
        public LineEvidence LineEvidence;
        public LineType PlotLineType = LineType.Mean;

        private ScatterSeries maxScatter;
        private ScatterSeries meanScatter;
        private ScatterSeries minScatter;
        private LineSeries minLine;
        private LineSeries maxLine;

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
                var value1 = Convert.ToDouble(valueSet[0]);
                var value2 = Convert.ToDouble(valueSet[1]);
                series2.Items.Add(new HighLowItem(index, value1, value2,
                    value1, value2));
            }
        }

        private void AddSectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (InputRows != null)
            {
                var row = new Dynamic();
                var sectionId = "Section " + (this.InputRows.Count + 1);
                row[CellModel.SectionIdHeader] = sectionId;
                this.LineEvidence.SectionEvidences.Add(new SectionEvidence {Id = sectionId});

                foreach (var year in this.LineEvidence.Years)
                {
                    row[year.ToString()] = null;
                }
                this.InputRows.Add(row);
            }
        }

        private VertexEvidenceProgress CheckVertexEvidenceProgress(Vertex vertex)
        {
            var total = this.LineEvidence.SectionEvidences.Count * this.LineEvidence.Years.Count();
            var sum = 0;

            foreach (var sectionEvidence in this.LineEvidence.SectionEvidences)
            {
                foreach (var year in this.LineEvidence.Years)
                {
                    var graphEvidence = sectionEvidence.YearEvidences[year].GraphEvidence;

                    if (graphEvidence.ContainsKey(vertex.Key))
                    {
                        sum++;
                    }
                }
            }

            if (sum == 0)
            {
                return VertexEvidenceProgress.None;
            }

            if (sum < total)
            {
                return VertexEvidenceProgress.Partial;
            }

            return VertexEvidenceProgress.Full;
        }

        private void ClearAllButton_Click(object sender, RoutedEventArgs e)
        {
            this.InputGridView.SelectAll();

            foreach (var cell in this.InputGridView.SelectedCells)
            {
                var cellModel = cell.ToModel();

                if (cellModel.IsColumnSectionId) continue;

                this.SetCell(cellModel, "");
            }

            this.InputGridView.UnselectAll();
        }

        private void CopyAcrossAll_Click(object sender, RoutedEventArgs e)
        {
            if (this.InputGridView.SelectedCells.Count == 1)
            {
                var model = new CellModel(this.InputGridView.SelectedCells[0]);
                var modelData = model.Data as VertexEvidence;
                if (!model.IsColumnSectionId && modelData != null)
                {
                    this.InputGridView.SelectAll();
                    foreach (var cell in this.InputGridView.SelectedCells)
                    {
                        var oldModel = new CellModel(cell);
                        if (!oldModel.IsColumnSectionId)
                        {
                            this.SetCell(oldModel, modelData.String);
                        }
                    }
                }
            }
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

            if (this.InputGridView.CurrentCell == null)
            {
                this.Notifications.Push(this.notificationBadCurrentCell);
                return;
            }

            var cellModel = this.InputGridView.CurrentCell.ToModel();

            if (cellModel.IsColumnSectionId)
            {
                this.Notifications.Push(this.notificationBadCurrentCell);
                return;
            }

            this.SetCell(cellModel, vertex);
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
            if (InputGridView.SelectedCells.Count == 1)
            {
                meanScatter = new ScatterSeries();
                meanScatter.MarkerFill = OxyColors.Green;
                var inputCandleStick = new CandleStickSeries();
                inputCandleStick.Color = OxyColors.Green;
                minScatter = new ScatterSeries();
                minLine = new LineSeries();
                minLine.Color = OxyColors.Blue;
                minScatter.MarkerFill = OxyColors.Blue;
                maxScatter = new ScatterSeries();
                maxLine = new LineSeries();
                maxLine.Color = OxyColors.Red;
                maxScatter.MarkerFill = OxyColors.Red;
                DataPlotModel = new PlotModel();

                var model = new CellModel(InputGridView.SelectedCells[0]);
                if (model.IsColumnSectionId)
                {
                    return;
                }
                if (IsYearPlot)
                {
                    AddPlotInfo(model.Year.ToString(), "Section ID");
                    foreach (var row in InputRows)
                    {
                        var rowIndex = InputRows.IndexOf(row) + 1;
                        var entry = "";
                        try
                        {
                            entry = row[model.Header].String;
                        }
                        catch (RuntimeBinderException e)
                        {
                            entry = row[model.Header];
                        }
                        if (!String.IsNullOrEmpty(entry))
                        {
                            AddPointsToPlot(entry, meanScatter, inputCandleStick, Convert.ToDouble(rowIndex));
                        }
                    }
                }
                else
                {
                    AddPlotInfo(model.SectionId, "Year");
                    var row = model.Row;
                    foreach (var column in InputGridView.Columns)
                    {
                        var year = column.Header.ToString();
                        var entry = row[year].ToString();

                        if (year != CellModel.SectionIdHeader && !String.IsNullOrEmpty(entry))
                        {
                            AddPointsToPlot(entry, meanScatter, inputCandleStick, Convert.ToDouble(year));
                        }
                    }
                }

                DataPlotModel.MouseDown += (s, e) =>
                {
                    var scatter = meanScatter;
                    if (PlotLineType == LineType.Max)
                    {
                        scatter = maxScatter;
                    }
                    else if (PlotLineType == LineType.Min)
                    {
                        scatter = minScatter;
                    }

                    if (e.ChangedButton == OxyMouseButton.Left)
                    {
                        var point = Axis.InverseTransform(e.Position, DataPlotModel.DefaultXAxis, DataPlotModel.DefaultYAxis);
                        point.X = (int) point.X;
                        scatter.Points.Add(new ScatterPoint(point.X, point.Y));
                        if (PlotLineType == LineType.Max)
                        {
                            maxLine.Points.Add(new DataPoint(point.X, point.Y));
                        }
                        else if (PlotLineType == LineType.Min)
                        {
                            minLine.Points.Add(new DataPoint(point.X, point.Y));
                        }
                        DataPlotModel.InvalidatePlot(true);
                    }
                    else if (e.ChangedButton == OxyMouseButton.Right)
                    {
                        var seriesCount = scatter.Points.Count;

                        if (seriesCount > 0)
                        {
                            scatter.Points.RemoveAt(seriesCount - 1);
                        }
                        if (PlotLineType == LineType.Max)
                        {
                            maxLine.Points.RemoveAt(maxLine.Points.Count - 1);
                        }
                        else if (PlotLineType == LineType.Min)
                        {
                            minLine.Points.RemoveAt(minLine.Points.Count - 1);
                        }
                    }
                    DataPlotModel.InvalidatePlot(true);
                };


                DataPlotModel.Series.Add(meanScatter);
                DataPlotModel.Series.Add(inputCandleStick);
                DataPlotModel.Series.Add(minLine);
                DataPlotModel.Series.Add(maxLine);
                DataPlotModel.Series.Add(maxScatter);
                DataPlotModel.Series.Add(minScatter);         
                DataPlotModel.Axes[0].IsZoomEnabled = false;
                DataPlotModel.InvalidatePlot(true);
            }
        }

        private void InterpolateButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataPlotModel != null && meanScatter.Points.Count > 1)
            {
                var interpolateList = new List<ScatterPoint>();
                var oldPoint = meanScatter.Points[0];
                for (var i = 1; i < meanScatter.Points.Count; i++)
                {
                    var newPoint = meanScatter.Points[i];
                    var pointSpacing = newPoint.X - oldPoint.X;
                    if ((pointSpacing) > 1)
                    {
                        var slope = (newPoint.Y - oldPoint.Y) / (pointSpacing);
                        for (var j = 1; j < pointSpacing; j++)
                        {
                            var addPoint = new ScatterPoint(oldPoint.X + j, newPoint.Y - (slope * (newPoint.X - (oldPoint.X + j))));
                            interpolateList.Add(addPoint);
                        }
                    }
                    oldPoint = newPoint;
                }
                meanScatter.Points.AddRange(interpolateList);
                DataPlotModel.InvalidatePlot(true);
                UploadToGrid(meanScatter);
            }
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.AddSectionButton.Click += AddSectionButton_Click;
            this.ClearAllButton.Click += this.ClearAllButton_Click;
            this.CreateInputButton.Click += CreateInputButton_Click;
            this.OpenButton.Click += OpenButton_Click;
            this.SaveButton.Click += SaveButton_Click;
            this.PlotButton.Click += PlotButton_Click;
            this.CopyAcrossColumns.Click += CopyAcrossColumns_Click;
            this.CopyAcrossRows.Click += CopyAcrossRows_Click;
            this.CopyAcrossAll.Click += CopyAcrossAll_Click;
            this.UploadFromPlot.Click += UploadFromPlot_Click;
            this.InterpolateButton.Click += InterpolateButton_Click;

            this.MeanButton.Checked += MeanButton_Checked;
            this.MinButton.Checked += MinButton_Checked;
            this.MaxButton.Checked += MaxButton_Checked;
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

            this.VertexControl.EvidenceEntered += this.GraphControl_EvidenceEntered;
        }

        private void MaxButton_Checked(object sender, RoutedEventArgs e)
        {
            PlotLineType = LineType.Max;
        }

        private void MeanButton_Checked(object sender, RoutedEventArgs e)
        {
            PlotLineType = LineType.Mean;
        }

        private void MinButton_Checked(object sender, RoutedEventArgs e)
        {
            PlotLineType = LineType.Min;
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

                    var graphEvidence = this.LineEvidence.SectionEvidences[cellModel.SectionId].YearEvidences[cellModel.Year].GraphEvidence;

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

        private void UploadFromPlot_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataPlotModel != null)
            {
                UploadToGrid(minScatter);
            }
        }

        private void UploadToGrid(ScatterSeries scatter)
        {
            if (IsYearPlot)
            {
                var year = this.DataPlotModel.Title;
                foreach (var point in scatter.Points)
                {
                    if (point.X > 0 && point.X <= this.InputRows.Count)
                    {
                        SetCell(new CellModel(this.InputRows[(int) point.X - 1], year), point.Y.ToString());
                    }
                }
            }
            else
            {
                var section = this.DataPlotModel.Title;
                var sectionIndex = Convert.ToInt32(section.Substring(8));

                foreach (var point in scatter.Points)
                {
                    var columns = this.InputGridView.Columns;
                    if (point.X >= Convert.ToInt32(columns[1].Header) && point.X <= Convert.ToInt32(columns[columns.Count - 1].Header)) ;
                    {
                        SetCell(new CellModel(this.InputRows[sectionIndex - 1], point.X.ToString()), point.Y.ToString());
                    }
                }
            }
        }
    }
}