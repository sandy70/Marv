﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Marv.Common;
using Marv.Common.Types;
using Microsoft.Win32;
using Telerik.Charting;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Calendar;
using Telerik.Windows.Controls.ChartView;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsx;
using Telerik.Windows.Documents.Spreadsheet.Model;
using GridViewColumn = Telerik.Windows.Controls.GridViewColumn;
using ICommand = Marv.Common.ICommand;

namespace Marv.Input
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        private const double Tolerance = 5;
        private static readonly NumericalAxis LinearAxis = new LinearAxis();
        private static readonly NumericalAxis LogarithmicAxis = new LogarithmicAxis();
        private readonly List<ICommand> commandStack = new List<ICommand>();
        private readonly List<Object> oldValues = new List<object>();
        private readonly List<GridViewCellClipboardEventArgs> pastedCells = new List<GridViewCellClipboardEventArgs>();
        private int addRowCommandsCount;
        private double baseTableMax = 100;
        private double baseTableMin;
        private double baseTableRange = 10;
        private int createdRowsCount;
        private GridViewColumn currentColumn;
        private int currentCommand;
        private DateSelectionMode dateSelectionMode = DateSelectionMode.Year;
        private List<DateTime> dates = new List<DateTime> { DateTime.Now };
        private ScatterDataPoint draggedPoint;
        private DateTime endDate = DateTime.Now;
        private Dict<string, string, InterpolationData> interpolationData = new Dict<string, string, InterpolationData>();
        private bool isCellToolbarEnabled;
        private bool isGraphControlVisible = true;
        private bool isGridViewReadOnly;
        private bool isInterpolateClicked;
        private bool isLineDataChartVisible = true;
        private bool isLineDataControlVisible = true;
        private bool isModelRun;
        private bool isTimelineToolbarVisible;
        private ILineData lineData;
        private Dict<DataTheme, string, EvidenceTable> lineDataObj = new Dict<DataTheme, string, EvidenceTable>();

        private Network network;
        private GridViewNewRowPosition newRowPosition = GridViewNewRowPosition.None;
        private NotificationCollection notifications = new NotificationCollection();
        private string requiredPercentiles;
        private string selectedColumnName;
        private InterpolationData selectedInterpolationData;
        private EvidenceRow selectedRow;
        private SummaryStatistic selectedStatistic;
        private DataTheme selectedTheme = DataTheme.User;
        private Vertex selectedVertex;
        private DateTime startDate = DateTime.Now;
        private EvidenceTable table;

        private Dict<DataTheme, string, Object> userDataObj = new Dict<DataTheme, string, Object>();
        private string userDataObjFileName;
        private NumericalAxis verticalAxis = LinearAxis;
        private bool isHeatMapVisible;

        public bool IsHeatMapVisible
        {
            get { return isHeatMapVisible; }
            set
            {
                isHeatMapVisible = value;
                this.RaisePropertyChanged();
            }
        }
        

        public int AddRowCommandsCount
        {
            get { return addRowCommandsCount; }
            set
            {
                addRowCommandsCount = value;
                this.RaisePropertyChanged();
            }
        }

        public double BaseTableMax
        {
            get { return this.baseTableMax; }
            set
            {
                if (value.Equals(this.baseTableMax))
                {
                    return;
                }

                this.baseTableMax = value;
                this.RaisePropertyChanged();
            }
        }

        public double BaseTableMin
        {
            get { return this.baseTableMin; }
            set
            {
                if (value.Equals(this.baseTableMin))
                {
                    return;
                }

                this.baseTableMin = value;
                this.RaisePropertyChanged();
            }
        }

        public double BaseTableRange
        {
            get { return this.baseTableRange; }
            set
            {
                if (value.Equals(this.baseTableRange))
                {
                    return;
                }

                this.baseTableRange = value;
                this.RaisePropertyChanged();
            }
        }

        public int CreatedRowsCount
        {
            get { return createdRowsCount; }
            set
            {
                createdRowsCount = value;
                this.RaisePropertyChanged();
            }
        }

        public GridViewColumn CurrentColumn
        {
            get { return this.currentColumn; }

            set
            {
                if (value.Equals(this.currentColumn))
                {
                    return;
                }

                this.currentColumn = value;
                this.RaisePropertyChanged();
            }
        }

        public int CurrentCommand
        {
            get { return this.currentCommand; }

            set
            {
                if (this.currentCommand == value)
                {
                    return;
                }

                this.currentCommand = value;
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

        public bool IsCellToolbarEnabled
        {
            get { return this.isCellToolbarEnabled; }
            set
            {
                this.isCellToolbarEnabled = value;
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

        public bool IsGridViewReadOnly
        {
            get { return this.isGridViewReadOnly; }
            set
            {
                if (value.Equals(this.isGridViewReadOnly))
                {
                    return;
                }
                this.isGridViewReadOnly = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsInterpolateClicked
        {
            get { return isInterpolateClicked; }
            set
            {
                isInterpolateClicked = value;
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

        public bool IsModelRun
        {
            get { return isModelRun; }
            set
            {
                isModelRun = value;
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

        public GridViewNewRowPosition NewRowPosition
        {
            get { return newRowPosition; }
            set
            {
                this.newRowPosition = value;
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

        public string RequiredPercentiles
        {
            get { return requiredPercentiles; }
            set
            {
                requiredPercentiles = value;
                this.RaisePropertyChanged();
            }
        }

        public string SelectedColumnName
        {
            get { return this.selectedColumnName; }

            set
            {
                this.selectedColumnName = value;
                this.RaisePropertyChanged();
            }
        }

        public InterpolationData SelectedInterpolationData
        {
            get { return this.selectedInterpolationData; }

            set
            {
                if (this.SelectedInterpolationData != null)
                {
                    this.SelectedInterpolationData.PropertyChanged -= SelectedInterpolationData_PropertyChanged;
                }

                this.selectedInterpolationData = value;
                this.RaisePropertyChanged();

                if (this.SelectedInterpolationData != null)
                {
                    this.SelectedInterpolationData.PropertyChanged += SelectedInterpolationData_PropertyChanged;
                }
            }
        }

        public SummaryStatistic SelectedStatistic
        {
            get { return selectedStatistic; }
            set
            {
                selectedStatistic = value;
                this.RaisePropertyChanged();
            }
        }

        public DataTheme SelectedTheme
        {
            get { return this.selectedTheme; }

            set
            {
                this.selectedTheme = value;
                this.RaisePropertyChanged();
            }
        }

        public Vertex SelectedVertex
        {
            get { return this.selectedVertex; }

            set
            {
                this.selectedVertex = value;
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

        public EvidenceTable Table
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

                this.table.CollectionChanged += table_CollectionChanged;
            }
        }

        public NumericalAxis VerticalAxis
        {
            get { return this.verticalAxis; }

            set
            {
                this.verticalAxis = value;
                this.RaisePropertyChanged();
            }
        }

        public MainWindow()
        {
            StyleManager.ApplicationTheme = new Windows8Theme();
            InitializeComponent();

            LinearAxis.SetBinding(NumericalAxis.MaximumProperty, new Binding { Source = this, Path = new PropertyPath("SelectedVertex.SafeMax") });
            LinearAxis.SetBinding(NumericalAxis.MinimumProperty, new Binding { Source = this, Path = new PropertyPath("SelectedVertex.SafeMin") });

            LogarithmicAxis.SetBinding(NumericalAxis.MaximumProperty, new Binding { Source = this, Path = new PropertyPath("SelectedVertex.SafeMax") });
            LogarithmicAxis.SetBinding(NumericalAxis.MinimumProperty, new Binding { Source = this, Path = new PropertyPath("SelectedVertex.SafeMin") });
        }

        protected void table_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var action = e.Action;

            if (action != NotifyCollectionChangedAction.Add)
            {
                return;
            }
            var command = new AddRowCommand(this.Table);

            this.AddRowCommandsCount++;

            this.UpdateCommandStack(command);
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            this.dates = new List<DateTime>();

            var date = this.StartDate;

            while (date < this.EndDate)
            {
                var incrementFuncs = new Dict<DateSelectionMode, Func<int, DateTime>>
                {
                    { DateSelectionMode.Year, date.AddYears },
                    { DateSelectionMode.Month, date.AddMonths },
                    { DateSelectionMode.Day, i => date.AddDays(i) }
                };

                this.dates.Add(date);
                date = incrementFuncs[this.DateSelectionMode](1);
            }

            this.dates.Add(date);

            this.lineDataObj[DataTheme.User] = new Dict<string, EvidenceTable>();

            this.UpdateTable();
        }

        private void AxisTypeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.UpdateVerticalAxis();
        }

        private void CaptureInterpolatedData(Dict<string, EvidenceTable> mergedDataSet)
        {
            if (this.SelectedInterpolationData == null)
            {
                return;
            }

            foreach (var kvp in this.interpolationData)
            {
                foreach (var column in kvp.Value)
                {
                    if (column.Value.Points == null)
                    {
                        continue;
                    }

                    this.SelectedInterpolationData = column.Value;

                    EvidenceTable interpolatedTable = null;

                    interpolatedTable = new EvidenceTable(this.dates);

                    foreach (var mergedEvidenceRow in mergedDataSet.Values[0])
                    {
                        var interpolatedRow = new EvidenceRow { From = mergedEvidenceRow.From, To = mergedEvidenceRow.To };
                        interpolatedTable.Add(interpolatedRow);
                    }

                    this.lineDataObj[DataTheme.Interpolated].Add(kvp.Key, interpolatedTable);

                    foreach (var interpolatedRow in interpolatedTable)
                    {
                        var midRangeValue = (interpolatedRow.From + interpolatedRow.To) / 2;

                        var evidenceString = this.SelectedInterpolationData.GetEvidenceString(midRangeValue);

                        interpolatedRow[this.selectedColumnName] = this.Network.Vertices[kvp.Key].States.ParseEvidenceString(evidenceString);
                    }
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.IsTimelineToolbarVisible = false;
        }

        private void CopyAcrossAll_Click(object sender, RoutedEventArgs e)
        {
            if (!this.SelectedTheme.Equals(DataTheme.User))
            {
                return;
            }
            var val = selectedRow[selectedColumnName];

            DateTime dateTime;
            if (selectedColumnName.TryParse(out dateTime))
            {
                foreach (var row in this.Table)
                {
                    foreach (var column in row.GetDynamicMemberNames())
                    {
                        row[column] = val;
                    }
                }
            }
        }

        private void CopyAcrossCol_Click(object sender, RoutedEventArgs e)
        {
            if (this.selectedColumnName == null || this.selectedRow == null)
            {
                return;
            }

            var val = selectedRow[this.selectedColumnName];

            DateTime dateTime;

            if (this.selectedColumnName.TryParse(out dateTime))
            {
                foreach (var row in this.Table)
                {
                    row[this.selectedColumnName] = val;
                }
            }
        }

        private void CopyAcrossRow_Click(object sender, RoutedEventArgs e)
        {
            if (this.selectedColumnName == null || this.selectedRow == null)
            {
                return;
            }

            var val = this.selectedRow[this.selectedColumnName];

            DateTime dateTime;

            if (this.selectedColumnName.TryParse(out dateTime))
            {
                foreach (var columnName in this.selectedRow.GetDynamicMemberNames())
                {
                    this.selectedRow[columnName] = val;
                }
            }
        }

        private void CreateNewBeliefDataSet()
        {
            if (this.lineDataObj[DataTheme.Beliefs].Count == 0)
            {
                var evidenceDateTime = this.lineDataObj[DataTheme.Merged].Values[0].DateTimes;
                var noOfEvidenceRows = this.lineDataObj[DataTheme.Merged].Values[0].Count;

                this.lineDataObj[DataTheme.Beliefs] = new Dict<string, EvidenceTable>();

                foreach (var vertex in this.Network.Vertices)
                {
                    var beliefTable = new EvidenceTable(evidenceDateTime);

                    var i = 0;
                    while (i <= noOfEvidenceRows - 1)
                    {
                        var beliefRow = new EvidenceRow
                        {
                            From = this.lineDataObj[DataTheme.Merged][0].Value[i].From,
                            To = this.lineDataObj[DataTheme.Merged][0].Value[i].To
                        };

                        beliefTable.Add(beliefRow);
                        i++;
                    }

                    this.lineDataObj[DataTheme.Beliefs].Add(vertex.Key, beliefTable);
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

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            var workbook = new Workbook();
            var worksheet = workbook.Worksheets.Add();
            worksheet.Name = this.SelectedVertex.Key;

            var template = new WorkSheetTemplate(worksheet);

            template.AddColumn("From", 0, 1);
            template.AddColumn("To", 0, 1);
            template.AddColumn("Mean", 0, this.Table.DateTimes.Count());
            template.AddColumn("Stdv", 0, this.Table.DateTimes.Count());

            var requiredPercentileList = new List<double>();
            if (!string.IsNullOrWhiteSpace(requiredPercentiles))
            {
                var parts = requiredPercentiles.Trim().Split("(),: ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                foreach (var part in parts)
                {
                    double value;
                    if (double.TryParse(part, out value))
                    {
                        requiredPercentileList.Add(value);
                    }
                }
            }

            requiredPercentileList.ForEach(val => template.AddColumn(val.ToString(), 0, this.Table.DateTimes.Count()));

            foreach (var kvp in template.GetColumns())
            {
                if (kvp.Key == "From" || kvp.Key == "To")
                {
                    continue;
                }

                foreach (var dateTime in this.Table.DateTimes)
                {
                    template.AddSubColumn(kvp.Key, dateTime.ToShortDateString(), 1, 1);
                }
            }

            var noOfDateTimes = this.Table.DateTimes.Count();
            var evidenceRowCount = this.lineDataObj[DataTheme.Beliefs].Values[0].Count;

            for (var row = 0; row < evidenceRowCount; row++)
            {
                worksheet.Cells[row + 2, 0].SetValue(this.lineDataObj[DataTheme.Beliefs][this.SelectedVertex.Key][row]["From"].ToString());
                worksheet.Cells[row + 2, 1].SetValue(this.lineDataObj[DataTheme.Beliefs][this.SelectedVertex.Key][row]["To"].ToString());

                for (var dateTime = 0; dateTime < noOfDateTimes; dateTime++)
                {
                    var colName = this.Table.DateTimes.ToList()[dateTime].String();
                    var beliefValue = (this.lineDataObj[DataTheme.Beliefs][this.SelectedVertex.Key][row][colName] as double[]);

                    var mean = selectedVertex.Mean(beliefValue);
                    var stdv = selectedVertex.StandardDeviation(beliefValue);
                    var percentiles = requiredPercentileList.Select(val => new VertexPercentileComputer(val).Compute(this.Network.Vertices[this.SelectedVertex.Key], beliefValue)).ToList();

                    worksheet.Cells[row + 2, template.GetSubColumnPosition("Mean",  row +2)].SetValue(mean);
                    worksheet.Cells[row + 2, template.GetSubColumnPosition("Stdv",  row +2)].SetValue(stdv);

                    requiredPercentileList.ForEach((val, i) => worksheet.Cells[row + 2, template.GetSubColumnPosition(val.ToString(), row+2)].SetValue(percentiles[i]));
                }
            }

            var dialog = new SaveFileDialog
            {
                Filter = Common.LineData.FileDescription + "|*." + "xlsx",
            };

            var result = dialog.ShowDialog();

            if (result != true)
            {
                return;
            }
            IWorkbookFormatProvider formatProvider = new XlsxFormatProvider();

            using (var output = new FileStream(dialog.FileName, FileMode.Create))
            {
                formatProvider.Export(workbook, output);
            }
        }

        private void Go_Click(object sender, RoutedEventArgs e)
        {
            this.Chart.AddNodeStateLines(this.SelectedVertex, BaseTableMax, BaseTableMin);
        }

        private void GraphControl_EvidenceEntered(object sender, VertexEvidence vertexEvidence)
        {
            if (this.selectedRow != null && this.SelectedColumnName != null && this.SelectedVertex != null)
            {
                var selectRow = this.lineDataObj[DataTheme.User][this.SelectedVertex.Key].FirstOrDefault(row => row.Equals(this.selectedRow));

                if (selectRow != null)
                {
                    selectRow[this.SelectedColumnName] = vertexEvidence;
                }
            }

            if (vertexEvidence == null)
            {
                // this.LineDataChart.RemoveUserEvidence(this.GetChartCategory());
                // this.LineDataControl.ClearSelectedCell();
            }
        }

        private void GraphControl_SelectionChanged(object sender, Vertex e)
        {
            if (this.SelectedVertex != null)
            {
                this.NewRowPosition = GridViewNewRowPosition.Bottom;
            }

            this.UpdateVerticalAxis();

            this.UpdateTable();

            if (this.SelectedColumnName != null)
            {
                this.SelectedInterpolationData = this.interpolationData[this.SelectedVertex.Key][this.SelectedColumnName];
            }

            this.Chart.Annotations.Remove(annotation => true);

            var columnName = this.CurrentColumn == null ? this.Table.DateTimes.First().String() : this.CurrentColumn.UniqueName;

            this.Plot(columnName);
        }

        private void LineDataImportExcelMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = Common.LineData.FileDescription + "|*." + "xlsx",
            };

            var result = dialog.ShowDialog();

            if (result != true)
            {
                return;
            }

            if (!File.Exists(dialog.FileName))
            {
                throw new FileNotFoundException(String.Format("File {0} was not found!", dialog.FileName));
            }

            IWorkbookFormatProvider formatProvider = new XlsxFormatProvider();
            Workbook workbook;
            using (var input = new FileStream(dialog.FileName, FileMode.Open))
            {
                workbook = formatProvider.Import(input);
            }

            var worksheet = workbook.Worksheets.GetByName(this.SelectedVertex.Key);

            var noOfRows = 0;
            var noOfColumns = 2 + this.Table.DateTimes.Count();

            while (worksheet.Cells[noOfRows, 0].GetValue().Value.ValueType != CellValueType.Empty)
            {
                noOfRows++;
            }

            for (var rowCount = 0; rowCount < noOfRows; rowCount++)
            {
                for (var colCount = 0; colCount < noOfColumns; colCount++)
                {
                    var val = worksheet.Cells[rowCount, colCount].GetValue().Value.GetValueAsString(new CellValueFormat("0.00E+00"));
                    Console.WriteLine(val);
                }
            }

            //foreach (var row in this.Table)
            //{
            //    row.From = Convert.ToDouble(worksheet.Cells[0, 0].GetValue().Value.RawValue);
            //    row.To = Convert.ToDouble(worksheet.Cells[0, 1].GetValue().Value.RawValue);
            //}
        }

        private void LineDataNewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.LineDataSaveAs();

            this.lineDataObj = new Dict<DataTheme, string, EvidenceTable>();
            this.BaseTableMax = 100;
            this.BaseTableMin = 0;
            this.BaseTableRange = 10;

            this.Chart.Annotations.Remove(annotation => true);
            this.Chart.AddNodeStateLines(this.SelectedVertex, this.BaseTableMax, this.BaseTableMin);
            this.SelectedVertex.IsUserEvidenceComplete = false;
            this.SelectedColumnName = null;
            this.SelectedInterpolationData = null;
            this.interpolationData = new Dict<string, string, InterpolationData>();
            this.SelectedTheme = DataTheme.User;
            this.UpdateTable();
        }

        private void LineDataOpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = Common.LineData.FileDescription + "|*." + Common.LineData.FileExtension,
                Multiselect = false
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            this.userDataObjFileName = dialog.FileName;

            var directoryName = Path.GetDirectoryName(dialog.FileName);

            if (Directory.Exists(Path.Combine(directoryName, "SectionBeliefs")) && Directory.Exists(Path.Combine(directoryName, "SectionEvidences")))
            {
                // This is a folder line data
                this.LineData = LineDataFolder.Read(dialog.FileName);
            }
            else
            {
                this.userDataObj = Common.Utils.ReadJson<Dict<DataTheme, string, Object>>(dialog.FileName);

                var userData = this.userDataObj[DataTheme.User];
                var storedInterpolationData = this.userDataObj[DataTheme.Interpolated];

                foreach (var kvp in userData)
                {
                    this.lineDataObj[DataTheme.User][kvp.Key] = kvp.Value as EvidenceTable;
                }

                this.dates = (List<DateTime>) this.lineDataObj[DataTheme.User][0].Value.DateTimes;
                this.UpdateTable();

                foreach (var kvp in storedInterpolationData)
                {
                    this.interpolationData[kvp.Key] = kvp.Value as Dict<string, InterpolationData>;
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

            if (result != true)
            {
                return;
            }

            this.userDataObjFileName = dialog.FileName;

            foreach (var kvp in this.lineDataObj[DataTheme.User])
            {
                this.userDataObj[DataTheme.User].Add(kvp.Key, kvp.Value);
            }
            foreach (var kvp in this.interpolationData)
            {
                this.userDataObj[DataTheme.Interpolated].Add(kvp.Key, kvp.Value);
            }

            this.userDataObj.WriteJson(this.userDataObjFileName);
        }

        private void LineDataSaveAsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.LineDataSaveAs();
        }

        private void LineDataSaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.userDataObjFileName == null)
            {
                this.LineDataSaveAs();
            }
            else
            {
                this.userDataObj.WriteJson(this.userDataObjFileName);
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.S || !Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                return;
            }

            if (this.userDataObjFileName == null)
            {
                this.LineDataSaveAs();
            }
            else
            {
                this.userDataObj.WriteJson(this.userDataObjFileName);
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
            try
            {
                if (this.SelectedTheme.Equals(DataTheme.User) || this.SelectedTheme.Equals(DataTheme.Interpolated))
                {
                    List<double> baseRowsList = null;

                    baseRowsList = Utils.CreateBaseRowsList(this.BaseTableMin, this.BaseTableMax, this.BaseTableRange);

                    var mergedDataSet = Utils.Merge(this.lineDataObj[DataTheme.User], baseRowsList, this.Network);

                    CaptureInterpolatedData(mergedDataSet);

                    mergedDataSet = mergedDataSet.UpdateWithInterpolatedData(this.lineDataObj[DataTheme.Interpolated]);

                    this.lineDataObj[DataTheme.Merged] = mergedDataSet;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Can run the model only for user or interpolated data set");
            }

            var vertexEvidences = new Dict<string, VertexEvidence>();
            var noOfEvidenceRows = this.lineDataObj[DataTheme.Merged].Values[0].Count;
            var noOfDateTimes = this.lineDataObj[DataTheme.Merged].Values[0].DateTimes.Count();

            for (var rowCount = 0; rowCount < noOfEvidenceRows; rowCount++)
            {
                for (var dateTimecount = 0; dateTimecount < noOfDateTimes; dateTimecount++)
                {
                    foreach (var kvp in this.lineDataObj[DataTheme.Merged])
                    {
                        var nodeKey = kvp.Key;
                        var evidenceTable = kvp.Value;

                        var dateTimeColumnName = evidenceTable[rowCount].GetDynamicMemberNames().ToList()[dateTimecount];
                        var evidence = evidenceTable[rowCount][dateTimeColumnName] as VertexEvidence;

                        vertexEvidences.Add(nodeKey, evidence);
                    }

                    var nodeBelief = this.Network.Run(vertexEvidences);

                    if (this.lineDataObj[DataTheme.Beliefs].Count == 0)
                    {
                        this.CreateNewBeliefDataSet();
                    }

                    foreach (var kvp in nodeBelief)
                    {
                        var nodeKey = kvp.Key;
                        var val = kvp.Value;

                        var beliefTable = this.lineDataObj[DataTheme.Beliefs][nodeKey];
                        var beliefRow = beliefTable[rowCount];

                        beliefRow[beliefRow.GetDynamicMemberNames().ToList()[dateTimecount]] = val;
                    }

                    vertexEvidences.Clear();
                }
            }
            this.SelectedTheme = DataTheme.Beliefs;
            MessageBox.Show("Model run sucessful");

            if (this.lineDataObj[DataTheme.Beliefs].Count > 0)
            {
                this.IsModelRun = true;
            }
        }

        private void RunSectionMenuItem_Click(object sender, RoutedEventArgs e) {}

        private void SelectedInterpolationData_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Type")
            {
                this.SelectedInterpolationData.CreatePoints(this.BaseTableMax, this.BaseTableMin, this.SelectedVertex.SafeMax, this.SelectedVertex.SafeMin);
            }
        }

        private void StartDateTimePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.StartDate > this.EndDate)
            {
                this.EndDate = this.StartDate;
            }
        }

        private void UpdateCommandStack(ICommand command)
        {
            if (this.commandStack.Count >= 100)
            {
                this.commandStack.RemoveAt(0);
            }

            this.commandStack.Add(command);
            this.CurrentCommand = this.commandStack.Count - 1;
        }

        private void UpdateTable()
        {
            if (this.SelectedVertex == null)
            {
                return;
            }

            
            this.GridView.CommitEdit();

            this.Table = this.lineDataObj[this.SelectedTheme][this.SelectedVertex.Key];
            

            if (this.Table == null || this.Table.Count == 0)
            {
                this.lineDataObj[this.SelectedTheme].Add(this.SelectedVertex.Key, this.Table = new EvidenceTable(this.dates));
            }

            this.SelectedColumnName = this.Table.DateTimes.First().String();
        }

        private void UpdateVerticalAxis()
        {
            if (this.SelectedVertex == null)
            {
                return;
            }

            this.VerticalAxis = this.SelectedVertex.AxisType == VertexAxisType.Linear ? LinearAxis : LogarithmicAxis;
            if (this.SelectedVertex.Type == VertexType.Labelled)
            {
                LinearAxis.Minimum = 1;
                LinearAxis.Maximum = this.SelectedVertex.States.Count;

                LogarithmicAxis.Minimum = 1;
                LogarithmicAxis.Maximum = this.SelectedVertex.States.Count;
            }

            else if (this.VerticalAxis.Equals(LogarithmicAxis))
            {
                LogarithmicAxis.Minimum = this.SelectedVertex.States[0].SafeMax / 10;
                LogarithmicAxis.Maximum = this.SelectedVertex.States[this.SelectedVertex.States.Count() - 1].SafeMin * 10;
            }

            else
            {
                LinearAxis.SetBinding(NumericalAxis.MaximumProperty, new Binding { Source = this, Path = new PropertyPath("SelectedVertex.SafeMax") });
                LinearAxis.SetBinding(NumericalAxis.MinimumProperty, new Binding { Source = this, Path = new PropertyPath("SelectedVertex.SafeMin") });

                LogarithmicAxis.SetBinding(NumericalAxis.MaximumProperty, new Binding { Source = this, Path = new PropertyPath("SelectedVertex.SafeMax") });
                LogarithmicAxis.SetBinding(NumericalAxis.MinimumProperty, new Binding { Source = this, Path = new PropertyPath("SelectedVertex.SafeMin") });
            }

            this.Chart.AddNodeStateLines(this.SelectedVertex, BaseTableMax, BaseTableMin);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void HeatMapMenuItem_Click(object sender, RoutedEventArgs e)
        {

            //foreach (var row in this.Table)
            //{
            //    foreach (var dateTime in this.Table.DateTimes)
            //    {
            //        this.GridView.CurrentCell.BeginEdit();
            //        this.GridView.CurrentCell.CommitEdit();
            //    }
            //}

            this.IsHeatMapVisible = true;
            
           
        }
    }
}