using System;
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
using System.Windows.Media.Imaging;
using Marv.Common;
using Marv.Common.Types;
using Microsoft.Win32;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Calendar;
using Telerik.Windows.Controls.ChartView;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsx;
using Telerik.Windows.Documents.Spreadsheet.Model;
using ExportExtensions = Telerik.Windows.Media.Imaging.ExportExtensions;
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
        private double adjustedSafeMax;
        private Dict<string, EvidenceTable> beliefsData = new Dict<string, EvidenceTable>();
        private List<string> columnNames = new List<string>();
        private string copiedColumnName;
        private int createdRowsCount;
        private GridViewColumn currentColumn;
        private int currentCommand;
        private DateSelectionMode dateSelectionMode = DateSelectionMode.Year;
        private List<DateTime> dates = new List<DateTime> { DateTime.Now };
        private string[,] evidenceStringArray;
        private bool isCellToolbarEnabled;
        private bool isCommentBlocksGridVisible;
        private bool isGraphControlVisible = true;
        private bool isGridViewReadOnly;
        private bool isHeatMapVisible;
        private bool isInterpolateClicked;
        private bool isLineDataChartVisible = true;
        private bool isLineDataControlVisible = true;
        private bool isModelRun;
        private bool isTimelineToolbarVisible;
        private ILineData lineData;
        private Network network;
        private GridViewNewRowPosition newRowPosition = GridViewNewRowPosition.None;
        private NotificationCollection notifications = new NotificationCollection();
        private LineData pipeLineData = new LineData();
        private string requiredPercentiles;
        private string selectedColumnName;
        private InterpolationData selectedInterpolationData;
        private EvidenceRow selectedRow;
        private DataTheme selectedTheme = DataTheme.User;
        private Vertex selectedVertex;
        private EvidenceTable table;
        private string userDataObjFileName;
        private NumericalAxis verticalAxis = LinearAxis;

        public int AddRowCommandsCount
        {
            get { return addRowCommandsCount; }
            set
            {
                addRowCommandsCount = value;
                this.RaisePropertyChanged();
            }
        }

        public double AdjustedSafeMax
        {
            get { return adjustedSafeMax; }
            set
            {
                adjustedSafeMax = value;
                this.RaisePropertyChanged();
            }
        }

        public Dict<string, EvidenceTable> BeliefsData
        {
            get { return beliefsData; }
            set
            {
                beliefsData = value;
                this.RaisePropertyChanged();
            }
        }

        public List<string> ColumnNames
        {
            get { return columnNames; }
            set
            {
                columnNames = value;
                this.RaisePropertyChanged();
            }
        }

        public string CopiedColumnName
        {
            get { return copiedColumnName; }
            set
            {
                copiedColumnName = value;
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

        public string[,] EvidenceStringArray
        {
            get { return evidenceStringArray; }
            set
            {
                evidenceStringArray = value;
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

        public bool IsCommentBlocksGridVisible
        {
            get { return isCommentBlocksGridVisible; }
            set
            {
                isCommentBlocksGridVisible = value;
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

        public bool IsHeatMapVisible
        {
            get { return isHeatMapVisible; }
            set
            {
                isHeatMapVisible = value;
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

        public LineData PipeLineData
        {
            get { return pipeLineData; }
            set
            {
                pipeLineData = value;
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

        public DataTheme SelectedTheme
        {
            get { return selectedTheme; }
            set
            {
                selectedTheme = value;
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

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            this.dates = new List<DateTime>();

            var date = this.PipeLineData.StartDate;

            while (date < this.PipeLineData.EndDate)
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

            this.PipeLineData.UserDataObj = new Dict<string, NodeData>();

            this.UpdateTable();
        }

        private void AxisTypeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.UpdateVerticalAxis();
        }

        private Dict<string, EvidenceTable> CaptureInterpolatedData(Dict<string, EvidenceTable> mergedDataSet)
        {
            var interpolatedData = new Dict<string, EvidenceTable>();

            //if (this.SelectedInterpolationData == null)
            //{
            //    return interpolatedData;
            //}

            foreach (var kvp in this.PipeLineData.UserDataObj)
            {
                var interpolatedTable = new EvidenceTable(this.dates);
                interpolatedData.Add(kvp.Key, interpolatedTable);

                foreach (var column in kvp.Value.InterpolatedNodeData)
                {
                    if (column.Value.Points == null)
                    {
                        continue;
                    }

                    this.SelectedInterpolationData = column.Value;

                    if (interpolatedTable.Count == 0)
                    {
                        foreach (var mergedEvidenceRow in mergedDataSet.Values[0])
                        {
                            var interpolatedRow = new EvidenceRow { From = mergedEvidenceRow.From, To = mergedEvidenceRow.To };
                            interpolatedTable.Add(interpolatedRow);
                        }
                    }

                    // interpolatedData.Add(kvp.Key, interpolatedTable);

                    foreach (var interpolatedRow in interpolatedTable)
                    {
                        var midRangeValue = (interpolatedRow.From + interpolatedRow.To) / 2;

                        var evidenceString = this.SelectedInterpolationData.GetEvidenceString(midRangeValue, this.VerticalAxis.Equals(LinearAxis)?VertexAxisType.Linear: VertexAxisType.Logarithmic);

                        interpolatedRow[column.Key] = this.Network.Vertices[kvp.Key].States.ParseEvidenceString(evidenceString);
                    }
                }

                Console.WriteLine();
            }

            return interpolatedData;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.IsTimelineToolbarVisible = false;
        }

        private void CommentBlocksGridView_CellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {
            var columnName = e.Cell.Column.UniqueName;
            var row = e.Cell.ParentRow.Item as EvidenceRow;

            var command = new CellEditCommand(row, columnName, this.SelectedVertex, e.NewData, e.OldData);
            command.Execute();

            this.UpdateCommandStack(command);
        }

        private void CommentBlocksGridView_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            var row = e.Row.Item as EvidenceRow;

            this.Chart.UpdateCommentBlocks(row, VerticalAxis);
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

        private void CopyLines_Click(object sender, RoutedEventArgs e)
        {
            this.CopiedColumnName = this.SelectedColumnName;
        }

        private void CreateNewBeliefDataSet(Dict<string, EvidenceTable> mergedDataSet)
        {
            if (this.BeliefsData.Count == 0 || this.BeliefsData[this.BeliefsData[0].Key].Count <
                mergedDataSet.Values[0].Count)
            {
                var evidenceDateTime = mergedDataSet.Values[0].DateTimes;
                var noOfEvidenceRows = mergedDataSet.Values[0].Count;

                this.BeliefsData = new Dict<string, EvidenceTable>();

                foreach (var vertex in this.Network.Vertices)
                {
                    var beliefTable = new EvidenceTable(evidenceDateTime);

                    var i = 0;
                    while (i < noOfEvidenceRows)
                    {
                        var beliefRow = new EvidenceRow
                        {
                            From = mergedDataSet[0].Value[i].From,
                            To = mergedDataSet[0].Value[i].To
                        };

                        beliefTable.Add(beliefRow);
                        i++;
                    }

                    this.BeliefsData.Add(vertex.Key, beliefTable);
                }
            }
        }

        private void DefineTimelineMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.IsTimelineToolbarVisible = true;
            this.IsCommentBlocksGridVisible = !this.IsCommentBlocksGridVisible;
        }

        private void EndDateTimePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.PipeLineData.StartDate > this.PipeLineData.EndDate)
            {
                this.PipeLineData.StartDate = this.PipeLineData.EndDate;
            }
        }

        private void ExportPlotButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "PNG Image|*.png",
            };

            var result = dialog.ShowDialog();

            if (result != true)
            {
                return;
            }

            using (Stream fileStream = File.Open(dialog.FileName, FileMode.OpenOrCreate))
            {
                ExportExtensions.ExportToImage(this.Chart, fileStream, new PngBitmapEncoder());
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
            var evidenceRowCount = this.BeliefsData.Values[0].Count;

            for (var row = 0; row < evidenceRowCount; row++)
            {
                worksheet.Cells[row + 2, 0].SetValue(this.BeliefsData[this.SelectedVertex.Key][row]["From"].ToString());
                worksheet.Cells[row + 2, 1].SetValue(this.BeliefsData[this.SelectedVertex.Key][row]["To"].ToString());

                for (var dateTime = 0; dateTime < noOfDateTimes; dateTime++)
                {
                    var colName = this.Table.DateTimes.ToList()[dateTime].String();
                    var evidence = (this.BeliefsData[this.SelectedVertex.Key][row][colName] as VertexEvidence);
                    var beliefValue = evidence.Value;
                    var mean = selectedVertex.Mean(beliefValue);
                    var stdv = selectedVertex.StandardDeviation(beliefValue);
                    var percentiles = requiredPercentileList.Select(val => new VertexPercentileComputer(val).Compute(this.Network.Vertices[this.SelectedVertex.Key], beliefValue)).ToList();

                    worksheet.Cells[row + 2, template.GetSubColumnPosition("Mean", row + 2)].SetValue(mean);
                    worksheet.Cells[row + 2, template.GetSubColumnPosition("Stdv", row + 2)].SetValue(stdv);

                    requiredPercentileList.ForEach((val, i) => worksheet.Cells[row + 2, template.GetSubColumnPosition(val.ToString(), row + 2)].SetValue(percentiles[i]));
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
            if (this.Network == null)
            {
                MessageBox.Show("Network file not loaded");
                return;
            }

            this.Chart.AddNodeStateLines(this.SelectedVertex, this.PipeLineData.BaseTableMax, this.PipeLineData.BaseTableMin,this.VerticalAxis);
        }

        private void GraphControl_EvidenceEntered(object sender, VertexEvidence vertexEvidence)
        {
            if (this.selectedRow != null && this.SelectedColumnName != null && this.SelectedVertex != null)
            {
                var selectRow = this.PipeLineData.UserDataObj[this.SelectedVertex.Key].UserTable.FirstOrDefault(row => row.Equals(this.selectedRow));

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
                this.SelectedInterpolationData = this.PipeLineData.UserDataObj[this.SelectedVertex.Key].InterpolatedNodeData[this.SelectedColumnName];
            }

            this.Chart.Annotations.Remove(annotation => true);

            var columnName = this.CurrentColumn == null ? this.Table.DateTimes.First().String() : this.CurrentColumn.UniqueName;

            this.Plot(columnName);
        }

        private void HeatMapMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.BeliefsData.Count == 0)
            {
                return;
            }
            var data = new List<PlotInfo>();

            var colNames = this.BeliefsData[this.SelectedVertex.Key].DateTimes.Select(dateTime => dateTime.ToShortDateString()).ToList();
            var rowNames = this.BeliefsData[this.SelectedVertex.Key].Select(row => row.From.ToString() + "-" + row.To.ToString()).ToList();

            for (var row = 0; row < this.Table.Count; row++)
            {
                for (var dateTime = 0; dateTime < this.BeliefsData[this.SelectedVertex.Key].DateTimes.Count(); dateTime++)
                {
                    var colName = this.Table.DateTimes.ToList()[dateTime].String();
                    var beliefValue = (this.BeliefsData[this.SelectedVertex.Key][row][colName] as VertexEvidence);

                    var pi = new PlotInfo
                    {
                        Row = rowNames[row],
                        Column = colNames[dateTime],
                        Value = selectedVertex.Mean(beliefValue.Params),
                    };

                    data.Add(pi);
                }
            }

            this.HeatMap.DataContext = data;

            this.IsHeatMapVisible = !this.IsHeatMapVisible;
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

            this.PipeLineData = new LineData();
            this.BeliefsData = new Dict<string, EvidenceTable>();
            this.Chart.Annotations.Remove(annotation => true);

            this.Chart.AddNodeStateLines(this.SelectedVertex, this.PipeLineData.BaseTableMax, this.PipeLineData.BaseTableMin,this.VerticalAxis);
            this.SelectedVertex.IsUserEvidenceComplete = false;
            this.SelectedColumnName = null;
            this.SelectedInterpolationData = null;
            this.IsModelRun = false;

            this.UpdateTable();
        }

        private void LineDataOpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = Common.LineData.FileDescription + "|*." + Common.LineData.FileExtension,
                Multiselect = false
            };

            if (dialog.ShowDialog() != true) {}

            this.userDataObjFileName = dialog.FileName;

            var directoryName = Path.GetDirectoryName(dialog.FileName);

            if (Directory.Exists(Path.Combine(directoryName, "SectionBeliefs")) && Directory.Exists(Path.Combine(directoryName, "SectionEvidences")))
            {
                // This is a folder line data
                this.LineData = LineDataFolder.Read(dialog.FileName);
            }
            else
            {
                this.PipeLineData = Common.Utils.ReadJson<LineData>(dialog.FileName);
                var deleteNodes = new List<string>();
                foreach (var kvp in this.PipeLineData.UserDataObj)
                {
                    if (!this.Network.Vertices.Keys.Contains(kvp.Key))
                    {
                        deleteNodes.Add(kvp.Key);
                    }
                }
                foreach (var key in deleteNodes)
                {
                    this.PipeLineData.UserDataObj.Remove(key);
                }

                foreach (var kvp in this.PipeLineData.UserDataObj)
                {
                    if (!kvp.Value.UserTable.Any())
                    {
                        continue;
                    }
                    this.dates = kvp.Value.UserTable.DateTimes.ToList();
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

            if (this.userDataObjFileName != null)
            {
                this.PipeLineData.WriteJson(this.userDataObjFileName);
            }
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
                this.PipeLineData.WriteJson(this.userDataObjFileName);
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
                this.PipeLineData.WriteJson(this.userDataObjFileName);
            }
        }

        private void PasteEverywhereButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.CopiedColumnName == null)
            {
                return;
            }

            var copiedInterpolationData = this.PipeLineData.UserDataObj[this.SelectedVertex.Key].InterpolatedNodeData[this.CopiedColumnName];
            if (copiedInterpolationData.Points == null)
            {
                return;
            }

            foreach (var kvp in this.PipeLineData.UserDataObj[this.SelectedVertex.Key].InterpolatedNodeData)
            {
                if (kvp.Key == this.CopiedColumnName)
                {
                    continue;
                }

                kvp.Value.Type = copiedInterpolationData.Type;
                kvp.Value.CreatePoints(this.PipeLineData.BaseTableMax, this.PipeLineData.BaseTableMin, this.SelectedVertex.SafeMax, this.SelectedVertex.SafeMin);

                foreach (var line in kvp.Value.Points.Where(line => line != null))
                {
                    line.Clear();
                }
            }

            foreach (var column in this.dates.Where(column => column.String() != this.CopiedColumnName))
            {
                for (var i = 0; i < copiedInterpolationData.Points.Count; i++)
                {
                    foreach (var point in copiedInterpolationData.Points[i])
                    {
                        var observableCollection = this.PipeLineData.UserDataObj[this.SelectedVertex.Key].InterpolatedNodeData[column.String()].Points;
                        if (observableCollection != null)
                        {
                            observableCollection[i].Add(point);
                        }
                    }
                }
            }
        }

        private void PasteLines_Click(object sender, RoutedEventArgs e)
        {
            if (this.CopiedColumnName == null)
            {
                return;
            }
            var copiedInterpolationData = this.PipeLineData.UserDataObj[this.SelectedVertex.Key].InterpolatedNodeData[this.CopiedColumnName];
            if (copiedInterpolationData.Points == null)
            {
                return;
            }
            var pastedInterpolationData = this.PipeLineData.UserDataObj[this.SelectedVertex.Key].InterpolatedNodeData[this.SelectedColumnName];

            pastedInterpolationData.Type = copiedInterpolationData.Type;
            pastedInterpolationData.CreatePoints(this.PipeLineData.BaseTableMax, this.PipeLineData.BaseTableMin, this.SelectedVertex.SafeMax, this.SelectedVertex.SafeMin);

            foreach (var line in pastedInterpolationData.Points.Where(line => line != null))
            {
                line.Clear();
            }

            for (var i = 0; i < copiedInterpolationData.Points.Count; i++)
            {
                foreach (var point in copiedInterpolationData.Points[i])
                {
                    pastedInterpolationData.Points[i].Add(point);
                }
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
            Dict<string, EvidenceTable> mergedDataSet;

            var baseRowsList = Utils.CreateBaseRowsList(this.PipeLineData.BaseTableMin, this.PipeLineData.BaseTableMax, this.PipeLineData.BaseTableRange);

            mergedDataSet = Utils.Merge(this.PipeLineData.UserDataObj, baseRowsList, this.Network);

            var interpolatedData = CaptureInterpolatedData(mergedDataSet);

            mergedDataSet = mergedDataSet.UpdateWithInterpolatedData(interpolatedData);

            var noOfEvidenceRows = mergedDataSet.Values[0].Count;

            for (var rowCount = 0; rowCount < noOfEvidenceRows; rowCount++)
            {
                var evidences = new Dict<DateTime, string, VertexEvidence>();

                foreach (var dateTime in mergedDataSet.Values[0].DateTimes)
                {
                    foreach (var kvp in mergedDataSet)
                    {
                        var nodeKey = kvp.Key;
                        var evidenceTable = kvp.Value;

                        if (evidenceTable.Count == 0)
                        {
                            continue;
                        }

                        var vertexEvidence = evidenceTable[rowCount][dateTime.String()] as VertexEvidence;

                        evidences[dateTime][nodeKey] = vertexEvidence;
                    }
                }

                var beliefs = this.Network.Run(evidences);

                if (this.BeliefsData.Count == 0 || this.BeliefsData[this.BeliefsData[0].Key].Count < noOfEvidenceRows)
                {
                    this.CreateNewBeliefDataSet(mergedDataSet);
                }

                foreach (var dateTime in mergedDataSet.Values[0].DateTimes)
                {
                    foreach (var kvp in beliefs[dateTime])
                    {
                        var nodeKey = kvp.Key;
                        var val = kvp.Value;

                        var beliefTable = this.BeliefsData[nodeKey];
                        var beliefRow = beliefTable[rowCount];

                        beliefRow[dateTime.String()] = this.Network.Vertices[nodeKey].States.ParseEvidenceString(val.ValueToDistribution());
                    }
                }
            }

            this.SelectedTheme = DataTheme.Beliefs;
            MessageBox.Show("Model run sucessful");

            if (this.BeliefsData.Count > 0)
            {
                this.IsModelRun = true;
            }
        }

        private void SelectedInterpolationData_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Type")
            {
                this.SelectedInterpolationData.CreatePoints(this.PipeLineData.BaseTableMax, this.PipeLineData.BaseTableMin, this.SelectedVertex.SafeMax, this.SelectedVertex.SafeMin);
            }
        }

        private void StartDateTimePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.PipeLineData.StartDate > this.PipeLineData.EndDate)
            {
                this.PipeLineData.EndDate = this.PipeLineData.StartDate;
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

            this.Table = this.SelectedTheme == DataTheme.User ? this.PipeLineData.UserDataObj[this.SelectedVertex.Key].UserTable : this.BeliefsData[this.SelectedVertex.Key];

            if (this.Table == null || this.Table.Count == 0)
            {
                this.Table = new EvidenceTable(this.dates);
                this.PipeLineData.UserDataObj.Add(this.SelectedVertex.Key, new NodeData { UserTable = this.Table });
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

                if (this.SelectedVertex.States[0].SafeMin == 0)
                {
                    LogarithmicAxis.Minimum = this.SelectedVertex.States[0].SafeMax / 100;
                   
                }
                else
                {
                    LogarithmicAxis.Minimum = this.SelectedVertex.States[0].SafeMin;
                }

                var intervals = selectedVertex.States.Select(state => state.Min).Concat(selectedVertex.States.Last().Max.Yield()).ToArray();
                if (intervals.Any(val => val == Double.PositiveInfinity))
                {
                    LogarithmicAxis.Maximum = this.SelectedVertex.States[this.SelectedVertex.States.Count() - 1].SafeMin * 10;
                }

                else
                {
                    LogarithmicAxis.Maximum = this.SelectedVertex.States[this.SelectedVertex.States.Count() - 1].SafeMax;
                }
            }
            else
            {
                var intervals = selectedVertex.States.Select(state => state.Min).Concat(selectedVertex.States.Last().Max.Yield()).ToArray();
                if (intervals.Any(val => val == Double.PositiveInfinity))
                {
                    LinearAxis.Maximum = this.SelectedVertex.States[this.SelectedVertex.States.Count() - 1].SafeMin * 10;
                }
                else
                {
                    LinearAxis.SetBinding(NumericalAxis.MaximumProperty, new Binding { Source = this, Path = new PropertyPath("SelectedVertex.SafeMax") });
                }

                LinearAxis.SetBinding(NumericalAxis.MinimumProperty, new Binding { Source = this, Path = new PropertyPath("SelectedVertex.SafeMin") });

                LogarithmicAxis.SetBinding(NumericalAxis.MaximumProperty, new Binding { Source = this, Path = new PropertyPath("SelectedVertex.SafeMax") });
                LogarithmicAxis.SetBinding(NumericalAxis.MinimumProperty, new Binding { Source = this, Path = new PropertyPath("SelectedVertex.SafeMin") });
            }

            this.Chart.AddNodeStateLines(this.SelectedVertex, this.PipeLineData.BaseTableMax, this.PipeLineData.BaseTableMin, this.VerticalAxis);
        }

        private void table_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}