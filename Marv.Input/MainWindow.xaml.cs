using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private const double Tolerance = 0.1;
        
        private readonly string oldColumnName;
        private readonly List<GridViewCellClipboardEventArgs> pastedCells = new List<GridViewCellClipboardEventArgs>();

        private string chartTitle;
        private GridViewColumn currentColumn;
        private DateSelectionMode dateSelectionMode = DateSelectionMode.Year;
        private List<DateTime> dates = new List<DateTime> { DateTime.Now };
        private ScatterDataPoint draggedPoint;
        private DateTime endDate = DateTime.Now;
        private Graph graph;
        private HorizontalAxisQuantity horizontalAxisQuantity = HorizontalAxisQuantity.Distance;
        private bool isCellToolbarEnabled;
        private bool isGraphControlVisible = true;
        private bool isGridViewReadOnly;
        private bool isLineDataChartVisible = true;
        private bool isLineDataControlVisible = true;
        private bool isTimelineToolbarVisible;
        private ILineData lineData;
        private Dict<DataTheme, string, EvidenceTable> lineDataObj = new Dict<DataTheme, string, EvidenceTable>();
        private string lineDataObjFileName;
        private double maximum = 100;
        private double minimum = 100;
        private Network network;
        private NotificationCollection notifications = new NotificationCollection();
        private string selectedColumnName;
        private EvidenceRow selectedRow;
        private string selectedSectionId;
        private DataTheme selectedTheme = DataTheme.User;
        private Vertex selectedVertex;
        private int selectedYear;
        private DateTime startDate = DateTime.Now;
        private EvidenceTable table;
        private ObservableCollection<ScatterDataPoint> userNumberPoints = new ObservableCollection<ScatterDataPoint>();

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

        private void Chart_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var mousePosition = e.GetPosition(this.Chart);
            var tuple = this.Chart.ConvertPointToData(mousePosition);

            var range = (double) tuple.FirstValue;
            var value = (double) tuple.SecondValue;

            var userDataPoint = new ScatterDataPoint
            {
                XValue = range,
                YValue = value
            };

            // Insert the userDataPoint in appropriate position
            var i = 0;

            while (i < this.UserNumberPoints.Count)
            {
                if (!(this.UserNumberPoints[i].XValue < userDataPoint.XValue))
                {
                    this.UserNumberPoints.Insert(i, userDataPoint);
                    return;
                }

                i++;
            }
            this.UserNumberPoints.Add(userDataPoint);
        }

        private void Chart_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.draggedPoint != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var chart = (RadCartesianChart) sender;

                var data = chart.ConvertPointToData(e.GetPosition(chart));

                this.draggedPoint.YValue = (double) (data.SecondValue);

                ScatterDataPoint replacePoint = null;

                foreach (var userPoint in this.userNumberPoints)
                {
                    if (userPoint.XValue.Equals(this.draggedPoint.XValue))
                    {
                        replacePoint = userPoint;
                    }
                }

                this.userNumberPoints.Replace(replacePoint, this.DraggedPoint);
            }
        }

        private void Chart_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(this.Chart);
            var data = this.Chart.ConvertPointToData(position);
            var firstUserPoint = this.UserNumberPoints.First();
            var lastUserPoint = this.UserNumberPoints.Last();

            var selectedDataPoint = new ScatterDataPoint
            {
                XValue = (double) data.FirstValue,
                YValue = (double) data.SecondValue
            };

            var deletePoints = new ObservableCollection<ScatterDataPoint>();
            foreach (var scatterDataPoint in this.UserNumberPoints.Except(firstUserPoint).Except(lastUserPoint))
            {
                if (Utils.Distance(selectedDataPoint, scatterDataPoint) < Tolerance)
                {
                    deletePoints.Add(scatterDataPoint);
                }
            }

            foreach (var scatterDataPoint in deletePoints)
            {
                this.userNumberPoints.Remove(scatterDataPoint);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.IsTimelineToolbarVisible = false;
        }

        private void CopyAcrossAll_Click(object sender, RoutedEventArgs e)
        {
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

            this.Chart.Annotations.Remove(annotation => true);

            var columnName = this.CurrentColumn == null ? this.oldColumnName : this.CurrentColumn.UniqueName;

            if (columnName == null)
            {
                return;
            }

            this.Plot(columnName);
        }

        /* private void GridView_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            this.Maximum = this.Table.GetMaximum();
            this.Minimum = this.Table.GetMinimum();
        }*/

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
                        ZIndex = 100
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
                    VerticalTo = this.SelectedVertex.SafeMax
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

            this.lineDataObj[DataTheme.Merged] = Utils.Merge(this.lineDataObj[DataTheme.User]);

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