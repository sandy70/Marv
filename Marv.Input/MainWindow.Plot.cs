using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using MoreLinq;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Telerik.Charting;
using Telerik.Windows.Controls.ChartView;
using DataPoint = OxyPlot.DataPoint;
using LinearAxis = Telerik.Windows.Controls.ChartView.LinearAxis;
using LineSeries = OxyPlot.Series.LineSeries;
using LogarithmicAxis = Telerik.Windows.Controls.ChartView.LogarithmicAxis;

namespace Marv.Input
{
    public partial class MainWindow
    {
        public enum LineType
        {
            Mode,
            Min,
            Max
        };

        public static readonly DependencyProperty AnchorPointsProperty =
            DependencyProperty.Register("AnchorPoints", typeof (ObservableCollection<CategoricalDataPoint>), typeof (MainWindow), new PropertyMetadata(new ObservableCollection<CategoricalDataPoint>()));

        public static readonly DependencyProperty IsChartEditEnabledProperty =
            DependencyProperty.Register("IsChartEditEnabled", typeof (bool), typeof (MainWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty MaxPointsProperty =
            DependencyProperty.Register("MaxPoints", typeof (ObservableCollection<CategoricalDataPoint>), typeof (MainWindow), new PropertyMetadata(new ObservableCollection<CategoricalDataPoint>()));

        public static readonly DependencyProperty MinPointsProperty =
            DependencyProperty.Register("MinPoints", typeof (ObservableCollection<CategoricalDataPoint>), typeof (MainWindow), new PropertyMetadata(new ObservableCollection<CategoricalDataPoint>()));

        public static readonly DependencyProperty ModePointsProperty =
            DependencyProperty.Register("ModePoints", typeof (ObservableCollection<CategoricalDataPoint>), typeof (MainWindow), new PropertyMetadata(new ObservableCollection<CategoricalDataPoint>()));

        public static readonly DependencyProperty VerticalAxisProperty =
            DependencyProperty.Register("VerticalAxis", typeof (CartesianAxis), typeof (MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty XTitleProperty =
            DependencyProperty.Register("XTitle", typeof (string), typeof (MainWindow), new PropertyMetadata(null));

        private readonly LinearAxis linearAxis = new LinearAxis();
        private readonly LogarithmicAxis logarightmicAxis = new LogarithmicAxis();

        private ScatterSeries inputScatter;
        private LineSeries maxLine;
        private ScatterSeries maxScatter;
        private LineSeries minLine;
        private ScatterSeries minScatter;
        private LineSeries modeLine;
        private ScatterSeries modeScatter;
        public LineType PlotLineType = LineType.Mode;

        public ObservableCollection<CategoricalDataPoint> AnchorPoints
        {
            get
            {
                return (ObservableCollection<CategoricalDataPoint>) GetValue(AnchorPointsProperty);
            }

            set
            {
                SetValue(AnchorPointsProperty, value);
            }
        }

        public bool IsChartEditEnabled
        {
            get
            {
                return (bool) GetValue(IsChartEditEnabledProperty);
            }
            set
            {
                SetValue(IsChartEditEnabledProperty, value);
            }
        }

        public ObservableCollection<CategoricalDataPoint> MaxPoints
        {
            get
            {
                return (ObservableCollection<CategoricalDataPoint>) GetValue(MaxPointsProperty);
            }
            set
            {
                SetValue(MaxPointsProperty, value);
            }
        }

        public ObservableCollection<CategoricalDataPoint> MinPoints
        {
            get
            {
                return (ObservableCollection<CategoricalDataPoint>) GetValue(MinPointsProperty);
            }
            set
            {
                SetValue(MinPointsProperty, value);
            }
        }

        public ObservableCollection<CategoricalDataPoint> ModePoints
        {
            get
            {
                return (ObservableCollection<CategoricalDataPoint>) GetValue(ModePointsProperty);
            }
            set
            {
                SetValue(ModePointsProperty, value);
            }
        }

        public CartesianAxis VerticalAxis
        {
            get
            {
                return (CartesianAxis) GetValue(VerticalAxisProperty);
            }

            set
            {
                SetValue(VerticalAxisProperty, value);
            }
        }

        public string XTitle
        {
            get
            {
                return (string) GetValue(XTitleProperty);
            }
            set
            {
                SetValue(XTitleProperty, value);
            }
        }

        private static void SortScatter(ScatterSeries scatter)
        {
            //scatter.Points.Sort(
            //    delegate(ScatterPoint p1, ScatterPoint p2) { return p1.X.CompareTo(p2.X); }
            //    );
        }

        private int GetAnchorIndex(CategoricalDataPoint point)
        {
            return this.AnchorPoints.IndexOf(anchorPoint => anchorPoint.Category.Equals(point.Category));
        }

        private void InitializeChart()
        {
            this.XTitle = this.IsYearPlot ? "Sections" : "Years";

            this.MaxPoints = new ObservableCollection<CategoricalDataPoint>();
            this.ModePoints = new ObservableCollection<CategoricalDataPoint>();
            this.MinPoints = new ObservableCollection<CategoricalDataPoint>();

            if (this.InputRows == null)
            {
                return;
            }

            var first = this.InputRows.First()[CellModel.SectionIdHeader] as string;
            var last = this.InputRows.Last()[CellModel.SectionIdHeader] as string;

            var maxValue = this.Graph.SelectedVertex.SafeMax * 0.9;
            var minValue = this.Graph.SelectedVertex.SafeMin == 0 ? 1 : this.Graph.SelectedVertex.SafeMin * 1.1;
            var modeValue = (maxValue + minValue) / 2;

            this.MaxPoints.Add(new CategoricalDataPoint
            {
                Category = first,
                Value = maxValue
            });
            this.MaxPoints.Add(new CategoricalDataPoint
            {
                Category = last,
                Value = maxValue
            });

            this.ModePoints.Add(new CategoricalDataPoint
            {
                Category = first,
                Value = modeValue
            });
            this.ModePoints.Add(new CategoricalDataPoint
            {
                Category = last,
                Value = modeValue
            });

            this.MinPoints.Add(new CategoricalDataPoint
            {
                Category = first,
                Value = minValue
            });
            this.MinPoints.Add(new CategoricalDataPoint
            {
                Category = last,
                Value = minValue
            });
        }

        private void InitializePlot()
        {
            //if (this.InputGridView.SelectedCells.Count != 1) return;
            //inputScatter = new ScatterSeries();
            //inputScatter.MarkerFill = OxyColors.Green;
            //inputScatter.Title = "Base";
            //var inputCandleStick = new CandleStickSeries();
            //inputCandleStick.Color = OxyColors.Green;
            //var candleStickSet = new Dictionary<double, CandleStickSeries>();
            //candleStickSet.Add(1, inputCandleStick);
            //minScatter = new ScatterSeries();
            //minScatter.MarkerFill = OxyColors.Blue;

            //minLine = new LineSeries();
            //minLine.Color = OxyColors.Blue;

            //maxScatter = new ScatterSeries();
            //maxScatter.MarkerFill = OxyColors.Blue;

            //maxLine = new LineSeries();
            //maxLine.Color = OxyColors.Blue;

            //modeScatter = new ScatterSeries();
            //modeScatter.Title = "Belief";
            //modeScatter.MarkerFill = OxyColors.Blue;
            //modeLine = new LineSeries();
            //modeLine.Color = OxyColors.Blue;

            //this.DataPlotModel = new PlotModel();

            //var sourceCellModel = this.InputGridView.SelectedCells[0].ToModel();

            //if (sourceCellModel.IsColumnSectionId) return;

            //var cellModels = this.IsYearPlot ? this.InputRows.ToCellModels(sourceCellModel.Header) : this.InputGridView.Columns.ToCellModels(sourceCellModel.Row);

            //cellModels.ForEach((cellModel, i) =>
            //{
            //    var index = this.IsYearPlot ? i + 1 : Convert.ToDouble(cellModel.Year);
            //    this.AddPointsToPlot(cellModel.Data as VertexData, this.inputScatter, candleStickSet, index);
            //});

            //var title = this.IsYearPlot ? sourceCellModel.Year.ToString() : sourceCellModel.SectionId;
            //var xLabel = this.IsYearPlot ? CellModel.SectionIdHeader : "Year";

            //this.AddPlotInfo(title, xLabel);

            //this.DataPlotModel.MouseDown += (s, e) =>
            //{
            //    var scatter = modeScatter;

            //    if (PlotLineType == LineType.Max) scatter = maxScatter;
            //    else if (PlotLineType == LineType.Min) scatter = minScatter;

            //    if (e.ChangedButton == OxyMouseButton.Left)
            //    {
            //        var point = Axis.InverseTransform(e.Position, this.DataPlotModel.DefaultXAxis, this.DataPlotModel.DefaultYAxis);

            //        if (DoesPointExist((int) point.X, point.Y, scatter) && isCorrectPointPosition((int) point.X, point.Y))
            //        {
            //            point.X = (int) point.X;
            //            scatter.Points.Add(new ScatterPoint(point.X, point.Y));

            //            this.DataPlotModel.InvalidatePlot(true);
            //        }
            //    }
            //    else if (e.ChangedButton == OxyMouseButton.Right)
            //    {
            //        var seriesCount = scatter.Points.Count;

            //        if (seriesCount > 0) scatter.Points.RemoveAt(seriesCount - 1);
            //    }
            //    if (PlotLineType == LineType.Max) UpdateLine(maxLine, scatter);
            //    else if (PlotLineType == LineType.Min) UpdateLine(minLine, scatter);
            //    else UpdateLine(modeLine, scatter);

            //    this.DataPlotModel.InvalidatePlot(true);
            //};

            //this.DataPlotModel.Series.Add(inputScatter);
            //foreach (var series in candleStickSet.Evidence)
            //{
            //    this.DataPlotModel.Series.Add(series);
            //}
            //this.DataPlotModel.Series.Add(minLine);
            //this.DataPlotModel.Series.Add(maxLine);
            //this.DataPlotModel.Series.Add(modeLine);
            //this.DataPlotModel.Series.Add(maxScatter);
            //this.DataPlotModel.Series.Add(minScatter);
            //this.DataPlotModel.Series.Add(modeScatter);

            //this.DataPlotModel.Axes[(int) PlotAxis.XAxis].IsZoomEnabled = false;
            //this.DataPlotModel.Axes[(int) PlotAxis.XAxis].IsPanEnabled = false;

            //this.DataPlotModel.LegendPlacement = LegendPlacement.Outside;

            //this.DataPlotModel.InvalidatePlot(true);

            //////////////////////////////
        }

        private void UpdateChart()
        {
            //if (this.InputGridView.SelectedCells.Count != 1) return;

            //var sourceCellModel = this.InputGridView.SelectedCells[0].ToModel();

            //var cellModels = this.IsYearPlot ? this.InputRows.ToCellModels(sourceCellModel.Header) : this.InputGridView.Columns.ToCellModels(sourceCellModel.Row);

            //this.AnchorPoints = new ObservableCollection<CategoricalDataPoint>();
            //this.baseDistributionSeries = new ObservableCollection<ObservableCollection<ProbabilityDataPoint>>();
            //this.BaseNumberPoints = new ObservableCollection<CategoricalDataPoint>();
            //this.BaseRangePoints = new ObservableCollection<RangeDataPoint>();

            //foreach (var cellModel in cellModels)
            //{
            //    this.AnchorPoints.Add(new CategoricalDataPoint {Category = cellModel.SectionId, Value = null});

            //    var vertexEvidence = cellModel.Data as VertexData;

            //    if (vertexEvidence == null) continue;

            //    var vertexEvidenceType = this.Graph.SelectedVertex.ParseEvidenceInfo(vertexEvidence.String);
            //    var paramValues = VertexData.ParseEvidenceParams(vertexEvidence.String);

            //    switch (vertexEvidenceType)
            //    {
            //        case VertexEvidenceType.Number:
            //        {
            //            this.BaseNumberPoints.Add(new CategoricalDataPoint
            //            {
            //                Category = cellModel.SectionId,
            //                Value = paramValues[0]
            //            });

            //            break;
            //        }

            //        case VertexEvidenceType.Range:
            //        {
            //            paramValues.Sort();

            //            while (this.baseDistributionSeries.Count < 2)
            //            {
            //                this.baseDistributionSeries.Add(new ObservableCollection<ProbabilityDataPoint>());
            //            }

            //            this.baseDistributionSeries[0].Add(new ProbabilityDataPoint
            //            {
            //                Category = cellModel.SectionId,
            //                Value = paramValues[0],
            //                Probability = 0
            //            });

            //            this.baseDistributionSeries[1].Add(new ProbabilityDataPoint
            //            {
            //                Category = cellModel.SectionId,
            //                Value = paramValues[1],
            //                Probability = 1
            //            });

            //            break;
            //        }

            //        case VertexEvidenceType.Distribution:
            //        {
            //            var maxProb = vertexEvidence.Evidence.Max();

            //            this.Graph.SelectedVertex.States.ForEach((state, i) =>
            //            {
            //                if (this.baseDistributionSeries.Count < i + 1)
            //                {
            //                    this.baseDistributionSeries.Add(new ObservableCollection<ProbabilityDataPoint>());
            //                }

            //                this.baseDistributionSeries[i].Add(new ProbabilityDataPoint
            //                {
            //                    Category = cellModel.SectionId,
            //                    Value = state.SafeMax - state.SafeMin,
            //                    Probability = vertexEvidence.Evidence[i] / maxProb
            //                });
            //            });

            //            break;
            //        }
            //    }
            //}
        }

        private void UpdateChartCellModels()
        {
            //if (this.InputGridView.SelectedCells.Count != 1) return;

            //var sourceCellModel = this.InputGridView.SelectedCells[0].ToModel();

            //this.ChartCellModels = this.IsYearPlot ? this.InputRows.ToCellModels(sourceCellModel.Header) : this.InputGridView.Columns.ToCellModels(sourceCellModel.Row);
        }

        private void UploadToGrid()
        {
            if (minScatter.Points.Count == 0 || modeScatter.Points.Count == 0 || maxScatter.Points.Count == 0)
            {
                return;
            }
            if (minScatter.Points.Count != maxScatter.Points.Count || modeScatter.Points.Count != maxScatter.Points.Count || minScatter.Points.Count != modeScatter.Points.Count)
            {
                return;
            }
            var tempMax = maxScatter;
            var tempMode = modeScatter;
            var tempMin = minScatter;
            SortScatter(tempMax);
            SortScatter(tempMode);
            SortScatter(tempMin);

            if (IsYearPlot)
            {
                var year = this.DataPlotModel.Title;
                for (var i = 0; i < modeScatter.Points.Count; i++)
                {
                    var evidenceString = "TRI(" + tempMin.Points[i].Y + "," + tempMode.Points[i].Y + "," + tempMax.Points[i].Y + ")";
                    if (tempMode.Points[i].X > 0 && tempMode.Points[i].X <= this.InputRows.Count)
                    {
                        SetCell(new CellModel(this.InputRows[(int) tempMode.Points[i].X - 1], year), evidenceString);
                    }
                }
            }
            else
            {
                var section = this.DataPlotModel.Title;
                var sectionIndex = Convert.ToInt32(section.Substring(8));

                for (var i = 0; i < modeScatter.Points.Count; i++)
                {
                    //var columns = this.InputGridView.Columns;
                    //var evidenceString = "TRI(" + tempMin.Points[i].Y + "," + tempMode.Points[i].Y + "," + tempMax.Points[i].Y + ")";
                    //if (tempMode.Points[i].X >= Convert.ToInt32(columns[1].Header) && tempMode.Points[i].X <= Convert.ToInt32(columns[columns.Count - 1].Header))
                    //{
                    //    SetCell(new CellModel(this.InputRows[sectionIndex - 1], tempMode.Points[i].X.ToString(CultureInfo.CurrentCulture)), evidenceString);
                    //}
                }
            }
        }
    }
}