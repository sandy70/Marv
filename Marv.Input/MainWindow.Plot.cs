using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using Marv.Common.Graph;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using ScatterPoint = OxyPlot.Series.ScatterPoint;

namespace Marv.Input
{
    public partial class MainWindow
    {
        public PlotModel DataPlotModel
        {
            get
            {
                return (PlotModel)GetValue(DataPlotModelProperty);
            }
            set
            {
                SetValue(DataPlotModelProperty, value);
            }
        }

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
            if (minScatter.Points.Count == 0 || modeScatter.Points.Count == 0 || maxScatter.Points.Count == 0) { return; }
            if (minScatter.Points.Count != maxScatter.Points.Count || modeScatter.Points.Count != maxScatter.Points.Count || minScatter.Points.Count != modeScatter.Points.Count) { return; }
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
            if (!IsLogarithmic)
            {
                this.DataPlotModel.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Left,
                    Title = "Input Data"
                });
            }
            else
            {
                this.DataPlotModel.Axes.Add(new LogarithmicAxis
                {
                    Position = AxisPosition.Left,
                    Title = "Input Data"
                });
            }
        }


        private void CheckForLogarithmicScale()
        {
            if (IsLogarithmic) { return; }
            var oldState = this.Graph.SelectedVertex.States[0];
            foreach (var state in this.Graph.SelectedVertex.States)
            {
                if (oldState == state) { continue; }
                if (!state.Max.Equals((oldState.Max * 10))) { return; }
                oldState = state;                                                        
            }
            IsLogarithmic = true;
        }

        private void AddPointsToPlot(VertexEvidence entry, ScatterSeries series1, Dictionary<double, CandleStickSeries> series2, double index)
        {        
            if (entry.ToString().Contains(","))
            {
                var probSet = entry.Evidence;
                CheckForLogarithmicScale();
                var i = 0;
                foreach (var state in this.Graph.SelectedVertex.States)
                {
                    var probValue = probSet[i];
                    var min = state.Min;
                    var max = state.Max;
                    if (max.Equals(Double.PositiveInfinity))
                    {
                        max = 2*min;
                    }

                    var probItem = new HighLowItem(index, min, max,
                        min, max);
                    if (series2.Keys.Contains(probValue))
                    {
                        series2[probValue].Items.Add(probItem);
                    }
                    else
                    {
                        var newSeries = new CandleStickSeries();
                        newSeries.Color = OxyColors.Green;
                        newSeries.CandleWidth = 10*probValue;
                        newSeries.Items.Add(probItem);                        
                        series2.Add(probValue, newSeries);
                    }
                    i++;
                }
                return;
            }
            if ((!entry.ToString().Contains(":")))
            {
                var value = Convert.ToDouble(entry.ToString());
                series1.Points.Add(new ScatterPoint(index, value));
            }
            else if (entry.ToString().Split(":".ToArray()).Length == 2)
            {
                var valueSet = entry.ToString().Split(":".ToArray());
                var value1 = Convert.ToDouble(valueSet[0]);
                var value2 = Convert.ToDouble(valueSet[1]);
                series2[1].Items.Add(new HighLowItem(index, value1, value2,
                    value1, value2));
            }
        }

        private void InitializePlot()
        {
            if (this.InputGridView.SelectedCells.Count != 1) { return; }
            inputScatter = new ScatterSeries();
            inputScatter.MarkerFill = OxyColors.Green;
            inputScatter.Title = "Base";
            var inputCandleStick = new CandleStickSeries();
            inputCandleStick.Color = OxyColors.Green;
            var candleStickSet = new Dictionary<double, CandleStickSeries>();
            candleStickSet.Add(1, inputCandleStick);
            minScatter = new ScatterSeries();
           
            minLine = new LineSeries();
            minLine.Color = OxyColors.Blue;
            minScatter.MarkerFill = OxyColors.Blue;
            maxScatter = new ScatterSeries();
            
            maxLine = new LineSeries();
            maxLine.Color = OxyColors.Blue;
            maxScatter.MarkerFill = OxyColors.Blue;
            modeScatter = new ScatterSeries();
            modeScatter.Title = "Belief";
            modeScatter.MarkerFill = OxyColors.Blue;
            modeLine = new LineSeries();
            modeLine.Color = OxyColors.Blue;

            this.DataPlotModel = new PlotModel();

            var model = new CellModel(InputGridView.SelectedCells[0]);
            if (model.IsColumnSectionId) { return; }
            if (IsYearPlot)
            {  
                foreach (var row in this.InputRows)
                {
                    var rowIndex = this.InputRows.IndexOf(row) + 1;
                    
                    var entry = row[model.Header];
                    
                    if (entry != null && !String.IsNullOrEmpty(entry.ToString()))
                    {
                        AddPointsToPlot(entry, inputScatter, candleStickSet, Convert.ToDouble(rowIndex));
                    }
                }
                AddPlotInfo(model.Year.ToString(CultureInfo.CurrentCulture), "Section ID");
            }
            else
            {                
                var row = model.Row;
                foreach (var column in this.InputGridView.Columns)
                {
                    var year = column.Header.ToString();
                    var entry = row[year] as VertexEvidence;

                    if (year != CellModel.SectionIdHeader && !String.IsNullOrEmpty(entry.ToString()))
                    {
                        AddPointsToPlot(entry, inputScatter, candleStickSet, Convert.ToDouble(year));
                    }
                }
                AddPlotInfo(model.SectionId, "Year");
            }

            this.DataPlotModel.MouseDown += (s, e) =>
            {
                var scatter = modeScatter;
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

                    if (seriesCount > 0)
                    {
                        scatter.Points.RemoveAt(seriesCount - 1);
                    }

                }
                if (PlotLineType == LineType.Max)
                {
                    UpdateLine(maxLine, scatter);
                }
                else if (PlotLineType == LineType.Min)
                {
                    UpdateLine(minLine, scatter);
                }
                else
                {
                    UpdateLine(modeLine, scatter);
                }
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
                if (xCoord == (int)point.X || xCoord <= 0)
                {
                    return false;
                }                  
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
