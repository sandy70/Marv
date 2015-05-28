using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Marv.Common;
using Marv.Common.Types;
using Marv.Controls;
using Microsoft.Win32;
using Telerik.Charting;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Calendar;
using Telerik.Windows.Controls.ChartView;
using Telerik.Windows.Controls.GridView;

namespace Marv.Input
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        private static DataColumn selectedColumn;
        private static DataRow selectedRow;
        private string chartTitle;
        private DataSet dataSet = new DataSet();
        private DateSelectionMode dateSelectionMode = DateSelectionMode.Year;
        private List<DateTime> dates;
        private DateTime endDate = DateTime.Now;
        private Graph graph;
        private HorizontalAxisQuantity horizontalAxisQuantity = HorizontalAxisQuantity.Distance;
        private bool isCellSelected;
        private bool isGraphControlVisible = true;
        private bool isLineDataChartVisible = true;
        private bool isLineDataControlVisible = true;
        private bool isTimelineToolbarVisible;
        private ILineData lineData;
        private string lineDataFileName;
        private LineDataSet lineDataSet = new LineDataSet();
        private Network network;
        private NotificationCollection notifications = new NotificationCollection();
        private string oldColumnName;
        private string selectedSectionId;
        private int selectedYear;
        private DateTime startDate = DateTime.Now;
        private DataTable table;
        private ObservableCollection<ScatterDataPoint> userNumberPoints = new ObservableCollection<ScatterDataPoint>();

        public string ChartTitle
        {
            get { return this.chartTitle; }

            set
            {
                if (value.Equals(this.chartTitle))
                {
                    return;
                }

                this.chartTitle = value;
                this.RaisePropertyChanged();
            }
        }

        public DateSelectionMode DateSelectionMode
        {
            get { return this.dateSelectionMode; }

            set
            {
                if (value.Equals(this.dateSelectionMode))
                {
                    return;
                }

                this.dateSelectionMode = value;
                this.RaisePropertyChanged();
            }
        }

        public DateTime EndDate
        {
            get { return this.endDate; }

            set
            {
                if (value.Equals(this.endDate))
                {
                    return;
                }

                this.endDate = value;
                this.RaisePropertyChanged();
            }
        }

        public Graph Graph
        {
            get { return this.graph; }

            set
            {
                if (value.Equals(this.graph))
                {
                    return;
                }

                this.graph = value;
                this.RaisePropertyChanged();
            }
        }

        public HorizontalAxisQuantity HorizontalAxisQuantity
        {
            get { return this.horizontalAxisQuantity; }

            set
            {
                if (value.Equals(this.horizontalAxisQuantity))
                {
                    return;
                }

                this.horizontalAxisQuantity = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsCellSelected
        {
            get { return this.isCellSelected; }
            set
            {
                isCellSelected = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsGraphControlVisible
        {
            get { return this.isGraphControlVisible; }

            set
            {
                if (value.Equals(this.isGraphControlVisible))
                {
                    return;
                }

                this.isGraphControlVisible = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsLineDataChartVisible
        {
            get { return this.isLineDataChartVisible; }

            set
            {
                if (value.Equals(this.isLineDataChartVisible))
                {
                    return;
                }

                this.isLineDataChartVisible = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsLineDataControlVisible
        {
            get { return this.isLineDataControlVisible; }

            set
            {
                if (value.Equals(this.isLineDataControlVisible))
                {
                    return;
                }

                this.isLineDataControlVisible = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsTimelineToolbarVisible
        {
            get { return this.isTimelineToolbarVisible; }

            set
            {
                if (value.Equals(this.isTimelineToolbarVisible))
                {
                    return;
                }

                this.isTimelineToolbarVisible = value;
                this.RaisePropertyChanged();
            }
        }

        public ILineData LineData
        {
            get { return this.lineData; }

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

        public LineDataSet LineDataSet
        {
            get { return lineDataSet; }
            set
            {
                lineDataSet = value;
                this.RaisePropertyChanged();
            }
        }

        public Network Network
        {
            get { return this.network; }

            set
            {
                if (value.Equals(this.network))
                {
                    return;
                }

                this.network = value;
                this.RaisePropertyChanged();
            }
        }

        public NotificationCollection Notifications
        {
            get { return this.notifications; }

            set
            {
                if (value.Equals(this.notifications))
                {
                    return;
                }

                this.notifications = value;
                this.RaisePropertyChanged();
            }
        }

        public string SelectedSectionId
        {
            get { return this.selectedSectionId; }

            set
            {
                if (value.Equals(this.selectedSectionId))
                {
                    return;
                }

                this.selectedSectionId = value;
                this.RaisePropertyChanged();
            }
        }

        public int SelectedYear
        {
            get { return this.selectedYear; }

            set
            {
                if (value.Equals(this.selectedYear))
                {
                    return;
                }

                this.selectedYear = value;
                this.RaisePropertyChanged();
            }
        }

        public DateTime StartDate
        {
            get { return this.startDate; }

            set
            {
                if (value.Equals(this.startDate))
                {
                    return;
                }

                this.startDate = value;
                this.RaisePropertyChanged();
            }
        }

        public DataTable Table
        {
            get { return this.table; }

            set
            {
                if (value != null && value.Equals(this.table))
                {
                    return;
                }

                this.table = value;
                this.RaisePropertyChanged();
            }
        }

        public ObservableCollection<ScatterDataPoint> UserNumberPoints
        {
            get { return this.userNumberPoints; }

            set
            {
                if (value.Equals(this.userNumberPoints))
                {
                    return;
                }

                this.userNumberPoints = value;
                this.RaisePropertyChanged();
            }
        }

        public MainWindow()
        {
            StyleManager.ApplicationTheme = new Windows8Theme();
            InitializeComponent();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            this.dates = new List<DateTime>();

            var date = this.StartDate;

            var incrementFuncs = new Dict<DateSelectionMode, Func<int, DateTime>>
            {
                { DateSelectionMode.Year, date.AddYears },
                { DateSelectionMode.Month, date.AddMonths },
                { DateSelectionMode.Day, i => date.AddDays(i) }
            };

            while (date < this.EndDate)
            {
                this.dates.Add(date);
                date = incrementFuncs[this.DateSelectionMode](1);
            }

            this.dates.Add(date);

            this.dataSet = new DataSet();

            this.UpdateTable();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.IsTimelineToolbarVisible = false;
        }

        private void CopyAcrossAll_OnClick(object sender, RoutedEventArgs e)
        {
            if (selectedRow != null)
            {
                if (selectedColumn != null)
                {
                    var val = selectedRow[selectedColumn];

                    if (selectedColumn.ColumnName != "From" && selectedColumn.ColumnName != "To")
                    {
                        foreach (DataRow row in this.Table.Rows)
                        {
                            foreach (DataColumn column in this.Table.Columns)
                            {
                                if (column.ColumnName != "From" && column.ColumnName != "To")
                                {
                                    row[column] = val;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void CopyAcrossCol_OnClick(object sender, RoutedEventArgs e)
        {
            if (selectedRow != null)
            {
                if (selectedColumn != null)
                {
                    var val = selectedRow[selectedColumn];

                    if (selectedColumn.ColumnName != "From" && selectedColumn.ColumnName != "To")
                    {
                        foreach (DataRow row in this.Table.Rows)
                        {
                            row[selectedColumn] = val;
                        }
                    }
                }
            }
        }

        private void CopyAcrossRow_OnClick(object sender, RoutedEventArgs e)
        {
            if (selectedRow != null)
            {
                if (selectedColumn != null)
                {
                    var val = selectedRow[selectedColumn];

                    if (selectedColumn.ColumnName != "From" && selectedColumn.ColumnName != "To")
                    {
                        foreach (DataColumn col in this.Table.Columns)
                        {
                            if (col.ColumnName != "From" && col.ColumnName != "To")
                            {
                                selectedRow[col] = val;
                            }
                        }
                    }
                }
            }
        }

        private void DefineTimelineMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.IsTimelineToolbarVisible = true;
        }

        private void EndDateTimePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.StartDate > this.EndDate)
            {
                this.StartDate = this.EndDate;
            }
        }

        private object GetChartCategory()
        {
            return this.HorizontalAxisQuantity == HorizontalAxisQuantity.Distance ? this.SelectedSectionId : this.SelectedYear as object;
        }

        private object GetChartCategory(string sectionId, int year)
        {
            return this.HorizontalAxisQuantity == HorizontalAxisQuantity.Distance ? sectionId : year as object;
        }

        private void GraphControl_EvidenceEntered(object sender, VertexEvidence vertexEvidence)
        {
            if (this.SelectedSectionId != null && this.SelectedYear > 0 && this.Graph.SelectedVertex != null)
            {
                this.LineData.GetEvidence(this.SelectedSectionId)[this.SelectedYear][this.Graph.SelectedVertex.Key] = vertexEvidence;
            }

            if (vertexEvidence == null)
            {
                // this.LineDataChart.RemoveUserEvidence(this.GetChartCategory());
                // this.LineDataControl.ClearSelectedCell();
            }
        }

        private void GraphControl_GraphChanged(object sender, ValueChangedEventArgs<Graph> e)
        {
            if (this.LineData == null)
            {
                this.LineData = new LineData();
                this.LineData.SetEvidence("Section 1", new Dict<int, string, VertexEvidence>());
            }
        }

        private void GraphControl_SelectionChanged(object sender, Vertex e)
        {
            this.UpdateTable();

            if (this.LineData != null)
            {
                var selectedVertex = this.Graph.SelectedVertex;

                //this.LineDataControl.ClearRows();

                //foreach (var sectionId in this.LineData.SectionIds)
                //{
                //    // var sectionEvidence = await this.LineData.GetEvidenceAsync(sectionId);

                //    if (selectedVertex != null)
                //    {
                //        //this.LineDataControl.AddRow(sectionId, sectionEvidence[null, selectedVertex.Key]);
                //    }
                //}

                if (selectedVertex == null)
                {
                    return;
                }

                var intervals = selectedVertex.Intervals.ToArray();

                //this.LineDataChart.SetVerticalAxis(selectedVertex.SafeMax, selectedVertex.SafeMin, intervals);

                //this.LineDataChart.SetUserEvidence(this.HorizontalAxisQuantity == HorizontalAxisQuantity.Distance
                //                                       ? this.LineData.GetEvidence(null, this.SelectedYear, this.Graph.SelectedVertex.Key)
                //                                       : this.LineData.GetEvidence(this.SelectedSectionId, null, this.Graph.SelectedVertex.Key));
            }
        }

        private void GridView_AddingNewDataItem(object sender, GridViewAddingNewEventArgs e)
        {
            e.OwnerGridViewItemsControl.CurrentColumn = e.OwnerGridViewItemsControl.Columns[0];
        }

        private void GridView_AutoGeneratingColumn(object sender, GridViewAutoGeneratingColumnEventArgs e)
        {
            var headerString = e.Column.Header as string;

            DateTime dateTime;

            if (headerString.TryParse(out dateTime))
            {
                e.Column.Header = new TextBlock
                {
                    Text = dateTime.ToShortDateString()
                };
            }
        }

        private void GridView_CellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {
            DateTime dateTime;

            var columnName = e.Cell.Column.UniqueName;

            // If this is a DateTime column
            if (columnName.TryParse(out dateTime))
            {
                this.Plot((e.Cell.ParentRow.DataContext as DataRowView).Row, columnName);
            }
        }

        private void GridView_CellValidating(object sender, GridViewCellValidatingEventArgs e)
        {
            var dataRowView = e.Cell.DataContext as DataRowView;
            var index = this.Table.Rows.IndexOf(dataRowView.Row);
            var colName = e.Cell.Column.UniqueName;
            var val = e.NewValue;

            var isvalid = IsCellDataValid(dataRowView, index, colName, val);

            if (!isvalid)
            {
                e.IsValid = false;
            }
        }

        private void GridView_CurrentCellChanged(object sender, GridViewCurrentCellChangedEventArgs e)
        {
            if (e.NewCell == null)
            {
                return;
            }

            var row = (e.NewCell.ParentRow.DataContext as DataRowView).Row;

            selectedRow = row;

            foreach (DataColumn col in this.Table.Columns)
            {
                if (col.ColumnName == e.NewCell.Column.UniqueName)
                {
                    selectedColumn = col;
                }
            }

            if (selectedColumn.ColumnName != "From" && selectedColumn.ColumnName != "To")
            {
                this.IsCellSelected = true;
            }
            else
            {
                this.IsCellSelected = false;
            }

            Console.WriteLine("Row: " + this.Table.Rows.IndexOf(row) + ", Column: " + e.NewCell.Column.UniqueName);
        }

        private void GridView_PastingCellClipboardContent(object sender, GridViewCellClipboardEventArgs e)
        {
            var dataRowView = e.Cell.Item as DataRowView;
            var colName = e.Cell.Column.UniqueName;

            if (e.Value != null)
            {
                var val = e.Value.ToString();

                if (colName != "From" && colName != "To")
                {
                    if (dataRowView.Row != null)
                    {
                        var vertexEvidence = this.Graph.SelectedVertex.States.ParseEvidenceString(val);
                        dataRowView.Row[colName] = vertexEvidence;
                    }
                }
            }
        }

        private bool IsCellDataValid(DataRowView dataRowView, int index, string colName, object val)
        {
            var isvalid = true;

            if (colName == "From" && index != -1)
            {
                if (this.Table.Rows.IndexOf(dataRowView.Row) == 0)
                {
                    var fromValue = Convert.ToDouble(val);
                    if (!DBNull.Value.Equals(dataRowView.Row["To"]))
                    {
                        var toValue = (double) dataRowView.Row["To"];

                        if (fromValue > toValue)
                        {
                            isvalid = false;
                        }
                    }
                }

                else
                {
                    var previousRow = this.Table.Rows[index - 1];

                    var fromValue = Convert.ToDouble(val);

                    if (!DBNull.Value.Equals(previousRow["To"]))
                    {
                        var previousToValue = (double) previousRow["To"];

                        if (fromValue < previousToValue && previousToValue != 0)
                        {
                            isvalid = false;
                        }
                    }
                    if (!DBNull.Value.Equals(dataRowView.Row["To"]))
                    {
                        var toValue = (double) dataRowView.Row["To"];

                        if (fromValue > toValue && toValue != 0)
                        {
                            isvalid = false;
                        }
                    }
                }
            }
            else if (colName == "To" && index != -1)
            {
                if (index == this.Table.Rows.Count - 1)
                {
                    var toValue = Convert.ToDouble(val);

                    if (!DBNull.Value.Equals(dataRowView.Row["From"]))
                    {
                        var fromValue = (double) dataRowView.Row["From"];

                        if (toValue < fromValue && fromValue != 0)
                        {
                            isvalid = false;
                        }
                    }
                }
                else
                {
                    var nextRow = this.Table.Rows[index + 1];

                    var toValue = Convert.ToDouble(val);

                    if (!DBNull.Value.Equals(nextRow["From"]))
                    {
                        var nextFromValue = (double) nextRow["From"];

                        if (toValue > nextFromValue && nextFromValue != 0)
                        {
                            isvalid = false;
                        }
                    }

                    if (!DBNull.Value.Equals(dataRowView.Row["From"]))
                    {
                        var fromValue = (double) dataRowView.Row["From"];

                        if (toValue < fromValue && fromValue != 0)
                        {
                            isvalid = false;
                        }
                    }
                }
            }
            else if (index == -1)
            {
                var lastRow = this.Table.Rows[this.Table.Rows.Count - 1];

                if (colName == "From" || colName == "To")
                {
                    var newValue = Convert.ToDouble(val);

                    if (!DBNull.Value.Equals(lastRow["To"]))
                    {
                        var previousToValue = (double) lastRow["To"];

                        if (newValue < previousToValue && previousToValue != 0)
                        {
                            isvalid = false;
                        }
                    }
                }
            }
            else
            {
                var vertexEvidence = this.Graph.SelectedVertex.States.ParseEvidenceString(val as string);

                if (vertexEvidence.Type == VertexEvidenceType.Invalid)
                {
                    isvalid = false;
                }
                else
                {
                    dataRowView.Row[colName] = vertexEvidence;
                }
            }

            return isvalid;
        }

        private void LineDataChart_EvidenceGenerated(object sender, EvidenceGeneratedEventArgs e)
        {
            var vertexEvidence = this.Graph.SelectedVertex.States.ParseEvidenceString(e.EvidenceString);

            var sectionId = this.HorizontalAxisQuantity == HorizontalAxisQuantity.Distance ? e.Category as string : this.SelectedSectionId;
            var year = this.HorizontalAxisQuantity == HorizontalAxisQuantity.Distance ? this.SelectedYear : (int) e.Category;

            if (vertexEvidence.Type != VertexEvidenceType.Invalid)
            {
                var sectionEvidence = this.LineData.GetEvidence(sectionId);
                sectionEvidence[year][this.Graph.SelectedVertex.Key] = vertexEvidence;
                this.LineData.SetEvidence(sectionId, sectionEvidence);

                // this.LineDataChart.SetUserEvidence(e.Category, vertexEvidence);
                //this.LineDataControl.SetEvidence(sectionId, year, vertexEvidence);
            }
        }

        private void LineDataChart_HorizontalAxisQuantityChanged(object sender, HorizontalAxisQuantity e)
        {
            //this.LineDataChart.SetUserEvidence(this.HorizontalAxisQuantity == HorizontalAxisQuantity.Distance
            //                                       ? this.LineData.GetEvidence(null, this.SelectedYear, this.Graph.SelectedVertex.Key)
            //                                       : this.LineData.GetEvidence(this.SelectedSectionId, null, this.Graph.SelectedVertex.Key));
            this.UpdateChartTitle();
        }

        private void LineDataOpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = Common.LineData.FileDescription + "|*." + Common.LineData.FileExtension,
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                this.lineDataFileName = dialog.FileName;

                var directoryName = Path.GetDirectoryName(dialog.FileName);

                if (Directory.Exists(Path.Combine(directoryName, "SectionBeliefs")) && Directory.Exists(Path.Combine(directoryName, "SectionEvidences")))
                {
                    // This is a folder line data
                    this.LineData = LineDataFolder.Read(dialog.FileName);
                }
                else
                {
                    this.LineData = Common.LineData.Read(dialog.FileName);
                }
            }
        }

        private void LineDataSaveAs()
        {
            var dialog = new SaveFileDialog
            {
                Filter = Common.LineData.FileDescription + "|*." + Common.LineData.FileExtension,
            };

            var result = dialog.ShowDialog();

            if (result == true)
            {
                this.lineDataFileName = dialog.FileName;
                this.LineData.Write(this.lineDataFileName);
            }
        }

        private void LineDataSaveAsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.LineDataSaveAs();
        }

        private void LineDataSaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.lineDataFileName == null)
            {
                this.LineDataSaveAs();
            }
            else
            {
                this.LineData.Write(this.lineDataFileName);
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.PropertyChanged -= MainWindow_PropertyChanged;
            this.PropertyChanged += MainWindow_PropertyChanged;
        }

        private void MainWindow_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LineData")
            {
                if (this.LineData != null)
                {
                    //this.LineDataControl.ClearRows();

                    //foreach (var sectionId in this.LineData.SectionIds)
                    //{
                    //    //this.LineDataControl.AddRow(sectionId, (await this.LineData.GetEvidenceAsync(sectionId))[null, this.Graph.SelectedVertex.Key]);
                    //}
                }
            }
        }

        private void Plot(DataRow dataRow, string columnName)
        {
            var from = (double) dataRow["From"];
            var to = (double) dataRow["To"];
            var vertexEvidence = dataRow[columnName] as VertexEvidence;

            if (vertexEvidence == null)
            {
                return;
            }

            // Remove older annotations
            this.Chart.Annotations.Remove(annotation => annotation.Tag == dataRow);

            // Add empty data point to initialize chart
            if (this.UserNumberPoints.Count == 0)
            {
                this.UserNumberPoints.Add(new ScatterDataPoint());
            }

            if (vertexEvidence.Type == VertexEvidenceType.Number)
            {
                this.Chart.Annotations.Add(new CartesianCustomLineAnnotation
                {
                    HorizontalFrom = @from,
                    HorizontalTo = to,
                    Stroke = new SolidColorBrush(Colors.Goldenrod),
                    StrokeThickness = 2,
                    Tag = dataRow,
                    VerticalFrom = vertexEvidence.Params[0],
                    VerticalTo = vertexEvidence.Params[0],
                });
            }
            else if (vertexEvidence.Type == VertexEvidenceType.Range)
            {
                this.Chart.Annotations.Add(new CartesianMarkedZoneAnnotation
                {
                    Fill = new SolidColorBrush(Colors.Goldenrod),
                    HorizontalFrom = @from,
                    HorizontalTo = to,
                    Tag = dataRow,
                    VerticalFrom = vertexEvidence.Params[0],
                    VerticalTo = vertexEvidence.Params[1],
                });
            }
            else
            {
                var maxValue = vertexEvidence.Value.Max();

                var selectedVertex = this.Graph.SelectedVertex;

                var fill = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(0, 1)
                };

                vertexEvidence.Value.ForEach((value, i) =>
                {
                    fill.GradientStops.Add(new GradientStop
                    {
                        Offset = selectedVertex.Intervals.ElementAt(i) / selectedVertex.SafeMax,
                        Color = Color.FromArgb((byte) (value / maxValue * 255), 218, 165, 32)
                    });

                    fill.GradientStops.Add(new GradientStop
                    {
                        Offset = selectedVertex.Intervals.ElementAt(i + 1) / selectedVertex.SafeMax,
                        Color = Color.FromArgb((byte) (value / maxValue * 255), 218, 165, 32)
                    });
                });

                this.Chart.Annotations.Add(new CartesianMarkedZoneAnnotation
                {
                    Fill = fill,
                    HorizontalFrom = @from,
                    HorizontalTo = to,
                    Tag = dataRow,
                    VerticalFrom = selectedVertex.SafeMin,
                    VerticalTo = selectedVertex.SafeMax
                });
            }
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null && propertyName != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void RunLineMenuItem_Click(object sender, RoutedEventArgs e)
        {
            foreach (var sectionId in this.LineData.SectionIds)
            {
                var sectionEvidence = this.lineData.GetEvidence(sectionId);
                var sectionBelief = this.Network.Run(sectionEvidence);

                this.lineData.SetBelief(sectionId, sectionBelief);
            }

            var mergedDataSet = this.lineDataSet.GetMergerdDataSet(this.dataSet);
        }

        private void RunSectionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var sectionEvidence = this.LineData.GetEvidence(this.SelectedSectionId);

            var sectionBelief = this.Network.Run(sectionEvidence);

            this.Graph.Belief = sectionBelief[this.SelectedYear];
            this.LineData.SetBelief(this.SelectedSectionId, sectionBelief);
        }

        private void StartDateTimePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.StartDate > this.EndDate)
            {
                this.EndDate = this.StartDate;
            }
        }

        private void UpdateChartTitle()
        {
            this.ChartTitle = this.HorizontalAxisQuantity == HorizontalAxisQuantity.Distance ? "Year: " + this.SelectedYear : "Section: " + this.SelectedSectionId;
        }

        private void UpdateTable()
        {
            if (this.Graph.SelectedVertex != null)
            {
                this.Table = this.dataSet.Tables[this.Graph.SelectedVertex.Key];

                if (this.Table == null)
                {
                    if (this.dates == null)
                    {
                        this.dates = new List<DateTime>
                        {
                            DateTime.Now
                        };
                    }

                    this.Table = new LineDataTable(this.Graph.SelectedVertex.Key);

                    this.Table.Columns.Add("From", typeof (double));
                    this.Table.Columns.Add("To", typeof (double));

                    foreach (var date in this.dates)
                    {
                        this.Table.Columns.Add(date.String(), typeof (VertexEvidence));
                    }

                    this.Table.Rows.Add(0, 0);

                    this.dataSet.Tables.Add(this.Table);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}