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
using System.Windows.Media;
using Marv.Common;
using Marv.Common.Types;
using Microsoft.Win32;
using Telerik.Charting;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Calendar;
using Telerik.Windows.Controls.ChartView;
using Telerik.Windows.Controls.GridView;
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
        private readonly Dict<string, string, InterpolationData> interpolationData = new Dict<string, string, InterpolationData>();
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
        private bool isCellToolbarEnabled;
        private bool isGraphControlVisible = true;
        private bool isGridViewReadOnly;
        private bool isInterpolateClicked;
        private bool isLineDataChartVisible = true;
        private bool isLineDataControlVisible = true;
        private bool isTimelineToolbarVisible;
        private ILineData lineData;
        private Dict<DataTheme, string, EvidenceTable> lineDataObj = new Dict<DataTheme, string, EvidenceTable>();
        private string lineDataObjFileName;

        private Network network;
        private GridViewNewRowPosition newRowPosition = GridViewNewRowPosition.None;
        private NotificationCollection notifications = new NotificationCollection();
        private string selectedColumnName;
        private InterpolationData selectedInterpolationData;
        private EvidenceRow selectedRow;
        private DataTheme selectedTheme = DataTheme.User;
        private Vertex selectedVertex;
        private DateTime startDate = DateTime.Now;
        private EvidenceTable table;
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

        private void LineDataNewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.LineDataSaveAs();

            this.lineDataObj = new Dict<DataTheme, string, EvidenceTable>();
            this.BaseTableMax = 100;
            this.BaseTableMin = 0;
            this.BaseTableRange = 10;

            this.Chart.Annotations.Remove(annotation => true);
            this.Chart.AddNodeStateLines(this.SelectedVertex, this.BaseTableMax, this.BaseTableMin);
            this.selectedVertex.IsUserEvidenceComplete = false;
            this.SelectedColumnName = null;
            this.UpdateTable();
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
                this.lineDataObjFileName = dialog.FileName;

                var directoryName = Path.GetDirectoryName(dialog.FileName);

                if (Directory.Exists(Path.Combine(directoryName, "SectionBeliefs")) && Directory.Exists(Path.Combine(directoryName, "SectionEvidences")))
                {
                    // This is a folder line data
                    this.LineData = LineDataFolder.Read(dialog.FileName);
                }
                else
                {
                    this.lineDataObj = Common.Utils.ReadJson<Dict<DataTheme, string, EvidenceTable>>(dialog.FileName);
                    this.dates = (List<DateTime>) this.lineDataObj[DataTheme.User][0].Value.DateTimes;
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
                this.lineDataObjFileName = dialog.FileName;

                this.lineDataObj.WriteJson(this.lineDataObjFileName);
            }
        }

        private void LineDataSaveAsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.LineDataSaveAs();
        }

        private void LineDataSaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.lineDataObjFileName == null)
            {
                this.LineDataSaveAs();
            }
            else
            {
                this.lineDataObj.WriteJson(this.lineDataObjFileName);
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.S || !Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                return;
            }

            if (this.lineDataObjFileName == null)
            {
                this.LineDataSaveAs();
            }
            else
            {
                this.lineDataObj.WriteJson(this.lineDataObjFileName);
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

                    var mergedDataSet = Utils.Merge(this.lineDataObj[this.SelectedTheme], baseRowsList, this.SelectedVertex, this.Network);

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
            var itr = this.lineDataObj[DataTheme.Merged].Values[0].DateTimes.GetEnumerator();

            var noOfDateTimes = 0;
            while (itr.MoveNext())
            {
                noOfDateTimes++;
            }

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

                        beliefRow[beliefRow.GetDynamicMemberNames().ToList()[dateTimecount]] = this.SelectedVertex.States.ParseEvidenceString(val.ValueToDistribution());
                    }

                    vertexEvidences.Clear();
                }
            }
            this.SelectedTheme = DataTheme.Beliefs;
            MessageBox.Show("Model run sucessful");
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
        }

        private void UpdateVerticalAxis()
        {
            if (this.SelectedVertex == null)
            {
                return;
            }

            this.VerticalAxis = this.SelectedVertex.AxisType == VertexAxisType.Linear ? LinearAxis : LogarithmicAxis;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}