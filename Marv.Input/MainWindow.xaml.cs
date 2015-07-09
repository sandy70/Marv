using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Marv.Common;
using Marv.Common.Interpolators;
using Marv.Common.Types;
using Marv.Controls;
using Microsoft.Win32;
using Telerik.Charting;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Calendar;
using Telerik.Windows.Controls.ChartView;
using GridViewColumn = Telerik.Windows.Controls.GridViewColumn;
using Path = System.IO.Path;

namespace Marv.Input
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        private const double Tolerance = 1;
        
        private readonly string oldColumnName;
        private readonly List<GridViewCellClipboardEventArgs> pastedCells = new List<GridViewCellClipboardEventArgs>();
        private double baseTableMax;
        private double baseTableMin;
        private double baseTableRange;
        private string chartTitle;
        private GridViewColumn currentColumn;
        private InterpolatorDataPoints currentInterpolatorDataPoints = new InterpolatorDataPoints();
        private DateSelectionMode dateSelectionMode = DateSelectionMode.Year;
        private List<DateTime> dates = new List<DateTime> { DateTime.Now };
        private ScatterDataPoint draggedPoint;
        private DateTime endDate = DateTime.Now;
        private Graph graph;
        private HorizontalAxisQuantity horizontalAxisQuantity = HorizontalAxisQuantity.Distance;
        private bool isBaseTableAvailable;
        private bool isCellToolbarEnabled;
        private bool isGraphControlVisible = true;
        private bool isGridViewReadOnly;
        private bool isLineDataChartVisible = true;
        private bool isLineDataControlVisible = true;
        private bool isTimelineToolbarVisible;
        private ILineData lineData;
        private Dict<DataTheme, string, EvidenceTable> lineDataObj = new Dict<DataTheme, string, EvidenceTable>();
        private string lineDataObjFileName;
        private double maxUserValue;
        private double maximum = 100;
        private double minUserValue;
        private double minimum = 100;
        private Network network;
        private NotificationCollection notifications = new NotificationCollection();
        private string selectedColumnName;
        private string selectedLine;
        private EvidenceRow selectedRow;
        private string selectedSectionId;
        private DataTheme selectedTheme = DataTheme.User;
        private Vertex selectedVertex;
        private int selectedYear;
        private DateTime startDate = DateTime.Now;
        private EvidenceTable table;
        private Dict<string, string, InterpolatorDataPoints> userNumberPoints;

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

        public InterpolatorDataPoints CurrentInterpolatorDataPoints
        {
            get { return this.currentInterpolatorDataPoints; }

            set
            {
                this.currentInterpolatorDataPoints = value;
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

        public ScatterDataPoint DraggedPoint
        {
            get { return this.draggedPoint; }

            set
            {
                this.draggedPoint = value;
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

        public double MaxUserValue
        {
            get { return this.maxUserValue; }

            set
            {
                if (this.maxUserValue.Equals(value))
                {
                    return;
                }

                this.maxUserValue = value;
                this.RaisePropertyChanged();
            }
        }

        public double Maximum
        {
            get { return this.maximum; }

            set
            {
                if (value.Equals(this.maximum))
                {
                    return;
                }

                this.maximum = value;
                this.RaisePropertyChanged();
            }
        }

        public double MinUserValue
        {
            get { return this.minUserValue; }

            set
            {
                if (this.minUserValue.Equals(value))
                {
                    return;
                }

                this.minUserValue = value;
                this.RaisePropertyChanged();
            }
        }

        public double Minimum
        {
            get { return this.minimum; }

            set
            {
                if (value.Equals(this.minimum))
                {
                    return;
                }

                this.minimum = value;
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

        public string SelectedLine
        {
            get { return this.selectedLine; }

            set
            {
                if (value.Equals(this.selectedLine))
                {
                    return;
                }
                this.selectedLine = value;
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
                if (value.Equals(this.selectedVertex))
                {
                    return;
                }

                this.selectedVertex = value;
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
            }
        }

        public Dict<string, string, InterpolatorDataPoints> UserNumberPoints
        {
            get { return this.userNumberPoints; }

            set
            {
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

        private void Chart_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.draggedPoint != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var chart = (RadCartesianChart) sender;

                var data = chart.ConvertPointToData(e.GetPosition(chart));

                this.DraggedPoint.YValue = (double) (data.SecondValue);

                ScatterDataPoint replacePoint = null;

                var currentLine = this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].GetNumberPoints(this.SelectedLine);

                foreach (var userPoint in currentLine)
                {
                    if (userPoint.XValue.Equals(this.DraggedPoint.XValue))
                    {
                        replacePoint = userPoint;
                    }
                }

                this.CurrentInterpolatorDataPoints.GetNumberPoints(this.SelectedLine).Replace(replacePoint, this.DraggedPoint);
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

        private void DeleteScatterDataPoint(ScatterDataPoint deletePoint)
        {
            var currentLine = this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].GetNumberPoints(this.SelectedLine);
            this.currentInterpolatorDataPoints = this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName];

            var closestPoint = currentLine[0];

            var closestPointDistance = Utils.Distance(closestPoint, deletePoint);

            foreach (var scatterDataPoint in currentLine)
            {
                closestPointDistance = Math.Min(closestPointDistance, Utils.Distance(scatterDataPoint, deletePoint));

                if (closestPointDistance < Tolerance)
                {
                    closestPoint = scatterDataPoint;
                    break;
                }
            }

            if (!(closestPointDistance < Tolerance))
            {
                return;
            }

            if (this.UserNumberPoints[this.SelectedVertex.Key] != null)
            {
                this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].GetNumberPoints(this.SelectedLine).Remove(closestPoint);
            }
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            var xCoordsMaximum = this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].GetNumberPoints("Maximum").GetXCoords();
            var yCoordsMaximum = this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].GetNumberPoints("Maximum").GetYCoords();

            var maxLinInterpolator = new LinearInterpolator(xCoordsMaximum, yCoordsMaximum);

            var xCoordsMode = this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].GetNumberPoints("Mode").GetXCoords();
            var yCoordsMode = this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].GetNumberPoints("Mode").GetYCoords();

            var modeLinInterpolator = new LinearInterpolator(xCoordsMode, yCoordsMode);

            var xCoordsMinimum = this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].GetNumberPoints("Minimum").GetXCoords();
            var yCoordsMinimum = this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].GetNumberPoints("Minimum").GetYCoords();

            var minLinInterpolator = new LinearInterpolator(xCoordsMinimum, yCoordsMinimum);

            EvidenceTable interpolatedTable = null;

            if (this.lineDataObj[DataTheme.Interpolated].Keys.Any(nodeKey => nodeKey.Equals(this.SelectedVertex.Key)))
            {
                interpolatedTable = this.lineDataObj[DataTheme.Interpolated][this.SelectedVertex.Key];
            }
            else
            {
                interpolatedTable = new EvidenceTable(this.dates);

                foreach (var userEvidenceRow in this.lineDataObj[DataTheme.User][this.SelectedVertex.Key])
                {
                    var interpolatedRow = new EvidenceRow { From = userEvidenceRow.From, To = userEvidenceRow.To };
                    interpolatedTable.Add(interpolatedRow);
                }
                this.lineDataObj[DataTheme.Interpolated].Add(this.SelectedVertex.Key, interpolatedTable);
            }

            foreach (var interpolatedRow in interpolatedTable)
            {
                var midRangeValue = (interpolatedRow.From + interpolatedRow.To) / 2;

                var yInterpolatedMin = Math.Round(minLinInterpolator.Eval(midRangeValue), 2);
                var yInterpolatedMode = Math.Round(modeLinInterpolator.Eval(midRangeValue), 2);
                var yInterpolatedMax = Math.Round(maxLinInterpolator.Eval(midRangeValue), 2);

                var val = "tri(" + yInterpolatedMin + "," + yInterpolatedMode + "," + yInterpolatedMax + ")";

                interpolatedRow[this.selectedColumnName] = this.SelectedVertex.States.ParseEvidenceString(val);
            }

            // Should currentInterpolator datapoints and usernumberpoints be cleared ???
        }

        private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.draggedPoint = ((sender as Ellipse).DataContext as ScatterDataPoint);
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
            this.isBaseTableAvailable = true;
        }

        private void GraphControl_EvidenceEntered(object sender, VertexEvidence vertexEvidence)
        {
            if (this.SelectedSectionId != null && this.SelectedYear > 0 && this.SelectedVertex != null)
            {
                this.LineData.GetEvidence(this.SelectedSectionId)[this.SelectedYear][this.SelectedVertex.Key] = vertexEvidence;
            }

            if (vertexEvidence == null)
            {
                // this.LineDataChart.RemoveUserEvidence(this.GetChartCategory());
                // this.LineDataControl.ClearSelectedCell();
            }
        }

        private void GraphControl_SelectionChanged(object sender, Vertex e)
        {
            this.UpdateTable();

            if (this.UserNumberPoints != null)
            {
                var vertexAvailable = this.UserNumberPoints.Keys.Any(key => key.Equals(this.SelectedVertex.Key));

                if (vertexAvailable)
                {
                    this.CurrentInterpolatorDataPoints = this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName];
                }
            }

            this.Chart.Annotations.Remove(annotation => true);

            var columnName = this.CurrentColumn == null ? this.oldColumnName : this.CurrentColumn.UniqueName;

            if (columnName == null)
            {
                return;
            }

            this.Plot(columnName);
        }

        private void InsertDataPoint(ScatterDataPoint userDataPoint)
        {
            this.CurrentInterpolatorDataPoints = this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName];

            var currentLine = this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].GetNumberPoints(this.SelectedLine);

            var index = currentLine.GetXCoords().IndexOf(xcoord => xcoord > userDataPoint.XValue);

            if (currentLine != null)
            {
                currentLine.Insert(index, userDataPoint);
            }
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
            this.LineDataSaveAs();
        }

        private void MaximumLine_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.SelectedLine = Utils.MaxInterpolatorLine;

            var mousePosition = e.GetPosition(this.Chart);
            var userDataPoint = this.Chart.GetScatterDataPoint(mousePosition);

            this.InsertDataPoint(userDataPoint);
        }

        private void MaximumLine_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.SelectedLine = Utils.MaxInterpolatorLine;
        }

        private void MaximumLine_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.SelectedLine = Utils.MaxInterpolatorLine;

            var position = e.GetPosition(this.Chart);
            var selectedDataPoint = this.Chart.GetScatterDataPoint(position);

            this.DeleteScatterDataPoint(selectedDataPoint);
        }

        private void MinimumLine_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.SelectedLine = Utils.MinInterpolatorLine;

            var mousePosition = e.GetPosition(this.Chart);
            var userDataPoint = this.Chart.GetScatterDataPoint(mousePosition);

            this.InsertDataPoint(userDataPoint);
        }

        private void MinimumLine_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.SelectedLine = Utils.MinInterpolatorLine;
        }

        private void MinimumLine_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.SelectedLine = Utils.MinInterpolatorLine;

            var selectedDataPoint = this.Chart.GetScatterDataPoint(e.GetPosition(this.Chart));

            this.DeleteScatterDataPoint(selectedDataPoint);
        }

        private void ModeLine_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.SelectedLine = Utils.ModeInterpolatorLine;

            var mousePosition = e.GetPosition(this.Chart);
            var userDataPoint = this.Chart.GetScatterDataPoint(mousePosition);

            this.InsertDataPoint(userDataPoint);
        }

        private void ModeLine_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.SelectedLine = Utils.ModeInterpolatorLine;
        }

        private void ModeLine_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.SelectedLine = Utils.ModeInterpolatorLine;

            var selectedDataPoint = this.Chart.GetScatterDataPoint(e.GetPosition(this.Chart));

            this.DeleteScatterDataPoint(selectedDataPoint);
        }

        private void Plot(string columnName)
        {
            foreach (var row in this.Table)
            {
                this.Plot(row, columnName);
            }
        }

        private void Plot(EvidenceRow dataRow, string columnName)
        {
            var fillBrush = new SolidColorBrush(Colors.Goldenrod);
            var strokeBrush = new SolidColorBrush(Colors.DarkGoldenrod);

            var from = (double) dataRow["From"];
            var to = (double) dataRow["To"];
            var vertexEvidence = dataRow[columnName] as VertexEvidence;

            if (vertexEvidence == null)
            {
                return;
            }

            // Remove older annotations
            this.Chart.Annotations.Remove(annotation => annotation.Tag.Equals(dataRow));

            if (vertexEvidence.Type == VertexEvidenceType.Number)
            {
                if (from == to)
                {
                    this.Chart.Annotations.Add(new CartesianCustomAnnotation
                    {
                        Content = new Ellipse
                        {
                            Fill = fillBrush,
                            Height = 8,
                            Stroke = strokeBrush,
                            Width = 8,
                        },
                        HorizontalAlignment = HorizontalAlignment.Center,
                        HorizontalValue = (from + to) / 2,
                        Tag = dataRow,
                        VerticalAlignment = VerticalAlignment.Center,
                        VerticalValue = vertexEvidence.Params[0],
                        ZIndex = -200
                    });
                }
                else
                {
                    this.Chart.Annotations.Add(new CartesianCustomLineAnnotation
                    {
                        HorizontalFrom = @from,
                        HorizontalTo = to,
                        Stroke = fillBrush,
                        StrokeThickness = 2,
                        Tag = dataRow,
                        VerticalFrom = vertexEvidence.Params[0],
                        VerticalTo = vertexEvidence.Params[0],
                        ZIndex = -200
                    });
                }
            }
            else if (vertexEvidence.Type == VertexEvidenceType.Range)
            {
                this.Chart.Annotations.Add(new CartesianMarkedZoneAnnotation
                {
                    Fill = fillBrush,
                    HorizontalFrom = @from,
                    HorizontalTo = to,
                    Stroke = strokeBrush,
                    Tag = dataRow,
                    VerticalFrom = vertexEvidence.Params[0],
                    VerticalTo = vertexEvidence.Params[1],
                    ZIndex = -200
                });
            }
            else if (vertexEvidence.Type != VertexEvidenceType.Null)
            {
                var maxValue = vertexEvidence.Value.Max();

                var fill = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(0, 1)
                };

                vertexEvidence.Value.ForEach((value, i) =>
                {
                    fill.GradientStops.Add(new GradientStop
                    {
                        Offset = this.SelectedVertex.Intervals.ElementAt(i) / this.SelectedVertex.SafeMax,
                        Color = Color.FromArgb((byte) (value / maxValue * 255), 218, 165, 32)
                    });

                    fill.GradientStops.Add(new GradientStop
                    {
                        Offset = this.SelectedVertex.Intervals.ElementAt(i + 1) / this.SelectedVertex.SafeMax,
                        Color = Color.FromArgb((byte) (value / maxValue * 255), 218, 165, 32)
                    });
                });

                this.Chart.Annotations.Add(new CartesianMarkedZoneAnnotation
                {
                    Fill = fill,
                    HorizontalFrom = @from,
                    HorizontalTo = to,
                    Stroke = strokeBrush,
                    Tag = dataRow,
                    VerticalFrom = this.SelectedVertex.SafeMin,
                    VerticalTo = this.SelectedVertex.SafeMax,
                    ZIndex = -200
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

            try
            {
                if (this.SelectedTheme.Equals(DataTheme.User) || this.SelectedTheme.Equals(DataTheme.Interpolated))
                {
                    List<double> baseRowsList = null;

                    if (this.isBaseTableAvailable)
                    {
                        baseRowsList = Utils.CreateBaseRowsList(this.BaseTableMin, this.BaseTableMax, this.BaseTableRange);
                    }

                    var mergedDataSet = Utils.Merge(this.lineDataObj[this.SelectedTheme], baseRowsList).UpdateInterpolatedData(this.lineDataObj[DataTheme.Interpolated]);
                    this.lineDataObj[DataTheme.Merged] = mergedDataSet;
                }
            }
            catch (Exception exception)
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

                        beliefRow[beliefRow.GetDynamicMemberNames().ToList()[dateTimecount]] = val;
                    }

                    vertexEvidences.Clear();
                }
            }

            // printing beliefDataSet
            foreach (var kvp in lineDataObj[DataTheme.Beliefs])
            {
                var beliefTable = kvp.Value;
                var nodeKey = kvp.Key;
                Console.WriteLine("Table={0}", nodeKey);
                foreach (var beliefRow in beliefTable)
                {
                    var colNameCollection = beliefRow.GetDynamicMemberNames().ToList();

                    foreach (var col in colNameCollection)
                    {
                        var valueArray = beliefRow[col] as double[];
                        Console.Write("[");
                        foreach (var val in valueArray)
                        {
                            Console.Write("{0},", Math.Round(val, 2));
                        }
                        Console.Write("]");
                    }

                    Console.Write(":");
                }
                Console.WriteLine("");
                Console.WriteLine("------------");
            }
        }

        private void RunSectionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var sectionEvidence = this.LineData.GetEvidence(this.SelectedSectionId);

            var sectionBelief = this.Network.Run(sectionEvidence);

            this.LineData.SetBelief(this.SelectedSectionId, sectionBelief);
        }

        private void StartDateTimePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.StartDate > this.EndDate)
            {
                this.EndDate = this.StartDate;
            }
        }

        private void UpdateTable()
        {
            if (this.SelectedVertex == null)
            {
                return;
            }

            this.Table = this.lineDataObj[this.SelectedTheme][this.SelectedVertex.Key];

            if (this.Table == null || this.Table.Count == 0)
            {
                this.lineDataObj[this.SelectedTheme].Add(this.SelectedVertex.Key, this.Table = new EvidenceTable(this.dates));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}