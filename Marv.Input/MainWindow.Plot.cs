using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;
using Marv.Common;
using Marv.Common.Graph;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Telerik.Charting;
using Telerik.Windows.Controls.ChartView;
using Axis = OxyPlot.Axes.Axis;
using DataPoint = OxyPlot.DataPoint;
using LinearAxis = OxyPlot.Axes.LinearAxis;
using LineSeries = OxyPlot.Series.LineSeries;
using LogarithmicAxis = OxyPlot.Axes.LogarithmicAxis;
using ScatterPoint = OxyPlot.Series.ScatterPoint;

namespace Marv.Input
{
    public partial class MainWindow
    {


        public enum PlotAxis
        {
            XAxis,
            YAxis
        }

        public enum LineType
        {
            Mode,
            Min,
            Max
        };

        public LineType PlotLineType = LineType.Mode;

        private ScatterSeries maxScatter;
        private ScatterSeries inputScatter;
        private ScatterSeries minScatter;
        private LineSeries minLine;
        private LineSeries maxLine;
        private ScatterSeries modeScatter;
        private LineSeries modeLine;

        private void UploadToGrid()
        {
            if (minScatter.Points.Count == 0 || modeScatter.Points.Count == 0 || maxScatter.Points.Count == 0) return; 
            if (minScatter.Points.Count != maxScatter.Points.Count || modeScatter.Points.Count != maxScatter.Points.Count || minScatter.Points.Count != modeScatter.Points.Count) return; 
            var tempMax = maxScatter;
            var tempMode = modeScatter;
            var tempMin = minScatter;
            SortScatter(tempMax);
            SortScatter(tempMode);
            SortScatter(tempMin);

            if (IsYearPlot)
            {
                var year = this.DataPlotModel.Title;
                for (var i = 0; i < modeScatter.Points.Count; i++ )
                {
                    var evidenceString = "TRI(" + tempMin.Points[i].Y + "," + tempMode.Points[i].Y + "," + tempMax.Points[i].Y + ")";
                    if (tempMode.Points[i].X > 0 && tempMode.Points[i].X <= this.InputRows.Count)
                    {
                        SetCell(new CellModel(this.InputRows[(int)tempMode.Points[i].X - 1], year), evidenceString);

                    }
                }
            }
            else
            {
                var section = this.DataPlotModel.Title;
                var sectionIndex = Convert.ToInt32(section.Substring(8));

                for (var i = 0; i < modeScatter.Points.Count; i++ )
                {
                    var columns = this.InputGridView.Columns;
                    var evidenceString = "TRI(" + tempMin.Points[i].Y + "," + tempMode.Points[i].Y + "," + tempMax.Points[i].Y + ")";
                    if (tempMode.Points[i].X >= Convert.ToInt32(columns[1].Header) && tempMode.Points[i].X <= Convert.ToInt32(columns[columns.Count - 1].Header)) 
                    {
                        SetCell(new CellModel(this.InputRows[sectionIndex - 1], tempMode.Points[i].X.ToString(CultureInfo.CurrentCulture)), evidenceString);
                    }
                }
            }
        }

        private static void SortScatter(ScatterSeries scatter)
        {
            scatter.Points.Sort(
                delegate(ScatterPoint p1, ScatterPoint p2)
                {
                    return p1.X.CompareTo(p2.X);
                }
                );    
        }

        private void AddPlotInfo(string title, string xAxis)
        {
            this.DataPlotModel.Title = title;

            this.DataPlotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = xAxis
            });

            if (this.Graph.SelectedVertex.IsLogScale)
            {
                this.DataPlotModel.Axes.Add(new LogarithmicAxis
                {
                    Position = AxisPosition.Left,
                    Title = "Input Data"
                });
            }
            else
            {
                this.DataPlotModel.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Left,
                    Title = "Input Data"
                });
            }
        }

        private void AddPointsToPlot(VertexEvidence evidence, ScatterSeries scatterSeries, Dictionary<double, CandleStickSeries> candleStickSeries, double index)
        {
            if (evidence == null || string.IsNullOrWhiteSpace(evidence.String)) return;

            if (evidence.String.Contains(","))
            {
                this.Graph.SelectedVertex.States.ForEach((state, i) =>
                {
                    var max = state.SafeMax;
                    var min = state.SafeMin;

                    var value = evidence.Values[i];

                    var highLowItem = new HighLowItem(index, min, max, min, max);

                    if (!candleStickSeries.ContainsKey(value))
                    {
                        candleStickSeries[value] = new CandleStickSeries
                        {
                            Color = OxyColors.Green,
                            CandleWidth = 10 * value
                        };
                    }

                    candleStickSeries[value].Items.Add(highLowItem);
                });

                return;
            }

            var values = VertexEvidence.ParseValues(evidence.String);

            if (evidence.String.Contains(":") && values.Count == 2)
            {
                candleStickSeries[1].Items.Add(new HighLowItem(index, values[0], values[1], values[0], values[1]));
            }
            else if (values.Count == 1)
            {
                scatterSeries.Points.Add(new ScatterPoint(index, values[0]));
            }
        }

        private void InitializePlot()
        {
            if (this.InputGridView.SelectedCells.Count != 1) return; 
            inputScatter = new ScatterSeries();
            inputScatter.MarkerFill = OxyColors.Green;
            inputScatter.Title = "Base";
            var inputCandleStick = new CandleStickSeries();
            inputCandleStick.Color = OxyColors.Green;
            var candleStickSet = new Dictionary<double, CandleStickSeries>();
            candleStickSet.Add(1, inputCandleStick);
            minScatter = new ScatterSeries();
            minScatter.MarkerFill = OxyColors.Blue;

            minLine = new LineSeries();
            minLine.Color = OxyColors.Blue;
            
            maxScatter = new ScatterSeries();
            maxScatter.MarkerFill = OxyColors.Blue;
            
            maxLine = new LineSeries();
            maxLine.Color = OxyColors.Blue;
            
            modeScatter = new ScatterSeries();
            modeScatter.Title = "Belief";
            modeScatter.MarkerFill = OxyColors.Blue;
            modeLine = new LineSeries();
            modeLine.Color = OxyColors.Blue;

            this.DataPlotModel = new PlotModel();

            var sourceCellModel = this.InputGridView.SelectedCells[0].ToModel();

            if (sourceCellModel.IsColumnSectionId) return;

            var cellModels = this.IsYearPlot ? this.InputRows.ToCellModels(sourceCellModel.Header) : this.InputGridView.Columns.ToCellModels(sourceCellModel.Row);

            cellModels.ForEach((cellModel, i) =>
            {
                var index = this.IsYearPlot ? i + 1 : Convert.ToDouble(cellModel.Year);
                this.AddPointsToPlot(cellModel.Data as VertexEvidence, inputScatter, candleStickSet, index);
            });

            var title = this.IsYearPlot ? sourceCellModel.Year.ToString() : sourceCellModel.SectionId;
            var xLabel = this.IsYearPlot ? CellModel.SectionIdHeader : "Year";

            this.AddPlotInfo(title, xLabel);

            this.DataPlotModel.MouseDown += (s, e) =>
            {
                var scatter = modeScatter;

                if (PlotLineType == LineType.Max) scatter = maxScatter;
                else if (PlotLineType == LineType.Min) scatter = minScatter;

                if (e.ChangedButton == OxyMouseButton.Left)
                {
                    var point = Axis.InverseTransform(e.Position, this.DataPlotModel.DefaultXAxis, this.DataPlotModel.DefaultYAxis);

                    if (DoesPointExist((int)point.X, point.Y, scatter) && isCorrectPointPosition((int)point.X, point.Y))
                    {
                        point.X = (int)point.X;
                        scatter.Points.Add(new ScatterPoint(point.X, point.Y));

                        this.DataPlotModel.InvalidatePlot(true);
                    }
                }
                else if (e.ChangedButton == OxyMouseButton.Right)
                {
                    var seriesCount = scatter.Points.Count;

                    if (seriesCount > 0) scatter.Points.RemoveAt(seriesCount - 1);
                }
                if (PlotLineType == LineType.Max) UpdateLine(maxLine, scatter);
                else if (PlotLineType == LineType.Min) UpdateLine(minLine, scatter);
                else  UpdateLine(modeLine, scatter);
               
                this.DataPlotModel.InvalidatePlot(true);
            };

            this.DataPlotModel.Series.Add(inputScatter);
            foreach (var series in candleStickSet.Values)
            {
                this.DataPlotModel.Series.Add(series);
            }
            this.DataPlotModel.Series.Add(minLine);
            this.DataPlotModel.Series.Add(maxLine);
            this.DataPlotModel.Series.Add(modeLine);
            this.DataPlotModel.Series.Add(maxScatter);
            this.DataPlotModel.Series.Add(minScatter);
            this.DataPlotModel.Series.Add(modeScatter);

            this.DataPlotModel.Axes[(int)PlotAxis.XAxis].IsZoomEnabled = false;
            this.DataPlotModel.Axes[(int)PlotAxis.XAxis].IsPanEnabled = false;
            

            this.DataPlotModel.LegendPlacement = LegendPlacement.Outside;

            this.DataPlotModel.InvalidatePlot(true);


            ///////////////////////////
            var count = 0;
            var finalCount = cellModels.Count();

            foreach (var cellModel in cellModels)
            {
                this.AnchorSeries.DataPoints.Add(new Telerik.Charting.CategoricalDataPoint { Category = cellModel.SectionId, Value = null });

                if (count == 0 || count == finalCount - 1)
                {
                    this.MaxSeries.DataPoints.Add(new Telerik.Charting.CategoricalDataPoint { Category = cellModel.SectionId, Value = 10 });
                    this.ModeSeries.DataPoints.Add(new Telerik.Charting.CategoricalDataPoint { Category = cellModel.SectionId, Value = 5 });
                    this.MinSeries.DataPoints.Add(new Telerik.Charting.CategoricalDataPoint { Category = cellModel.SectionId, Value = 1 });
                }

                count++;
            }

            this.Chart.MouseDown -= Chart_MouseDown;
            this.Chart.MouseDown += Chart_MouseDown;

            this.TrackBallBehavior.TrackInfoUpdated += TrackBallBehavior_TrackInfoUpdated;
        }

        void TrackBallBehavior_TrackInfoUpdated(object sender, TrackBallInfoEventArgs e)
        {
            Console.WriteLine(e.Context.TouchLocation);
        }

        void Chart_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(this.Chart);
            var data = this.Chart.ConvertPointToData(position);
            var index = this.AnchorSeries.DataPoints.IndexOf<CategoricalDataPoint>(point => point.Category == data.FirstValue);
            int insertIndex = -1;

            foreach (var maxPoint in this.MaxSeries.DataPoints)
            {
                var maxPointIndex = this.AnchorSeries.DataPoints.IndexOf<CategoricalDataPoint>(point => point.Category == maxPoint.Category);

                if (maxPointIndex >= index)
                {
                    insertIndex = this.MaxSeries.DataPoints.IndexOf(maxPoint);
                    break;
                }
            }

            this.MaxSeries.DataPoints.Insert(insertIndex, new CategoricalDataPoint {Category = data.FirstValue, Value = data.SecondValue as double?});

            Console.WriteLine(data.FirstValue + ", " + data.SecondValue + ", " + index + ", " + insertIndex);
        }

        private static void UpdateLine(LineSeries line, ScatterSeries scatter)
        {
            line.Points.Clear();

            foreach (var point in scatter.Points)
            {
                line.Points.Add(new DataPoint(point.X, point.Y));
            }

            line.Points.Sort(
                delegate(DataPoint p1, DataPoint p2)
                {
                    return p1.X.CompareTo(p2.X);
                }
                );
        }

        private bool DoesPointExist(int xCoord, double yCoord, ScatterSeries series)
        {
            foreach (var point in series.Points)
            {
                if (xCoord == (int)point.X || xCoord <= 0) return false;               
            }
            return true;
        }

        private bool isCorrectPointPosition(int xCoord, double yCoord)
        {
            if (PlotLineType == LineType.Mode)
            {
                foreach (var maxP in maxScatter.Points)
                {
                    if (xCoord == (int)maxP.X && yCoord > maxP.Y) { return false; }
                }
                foreach (var minP in minScatter.Points)
                {
                    if (xCoord == (int)minP.X && yCoord < minP.Y) { return false; }
                }
            }
            else if (PlotLineType == LineType.Min)
            {
                foreach (var maxP in maxScatter.Points)
                {
                    if (xCoord == (int)maxP.X && yCoord > maxP.Y) { return false; }
                }
                foreach (var modeP in modeScatter.Points)
                {
                    if (xCoord == (int)modeP.X && yCoord > modeP.Y) { return false; }
                }
            }
            else
            {
                foreach (var minP in minScatter.Points)
                {
                    if (xCoord == (int)minP.X && yCoord < minP.Y) { return false; }
                }
                foreach (var modeP in modeScatter.Points)
                {
                    if (xCoord == (int)modeP.X && yCoord < modeP.Y) { return false; }
                }
            }
            return true;
        }
    }
}
