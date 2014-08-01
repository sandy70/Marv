using System;
using System.Linq;
using Microsoft.CSharp.RuntimeBinder;
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

        public enum LineType
        {
            Median,
            Min,
            Max
        };

        public LineType PlotLineType = LineType.Median;

        private ScatterSeries _maxScatter;
        private ScatterSeries _inputScatter;
        private ScatterSeries _minScatter;
        private LineSeries _minLine;
        private LineSeries _maxLine;
        private ScatterSeries _medianScatter;
        private LineSeries _medianLine;

        private void UploadToGrid(ScatterSeries scatter)
        {
            if (IsYearPlot)
            {
                var year = this.DataPlotModel.Title;
                foreach (var point in scatter.Points)
                {
                    if (point.X > 0 && point.X <= this.InputRows.Count)
                    {
                        SetCell(new CellModel(this.InputRows[(int)point.X - 1], year), point.Y.ToString());
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

        private void InitializePlot()
        {
            if (this.InputGridView.SelectedCells.Count != 1) { return; }
            _inputScatter = new ScatterSeries();
            _inputScatter.MarkerFill = OxyColors.Green;
            _inputScatter.Title = "Base";
            var inputCandleStick = new CandleStickSeries();
            inputCandleStick.Color = OxyColors.Green;
            _minScatter = new ScatterSeries();
            _minScatter.Title = "Minimum";
            _minLine = new LineSeries();
            _minLine.Color = OxyColors.Blue;
            _minScatter.MarkerFill = OxyColors.Blue;
            _maxScatter = new ScatterSeries();
            _maxScatter.Title = "Maximum";
            _maxLine = new LineSeries();
            _maxLine.Color = OxyColors.Red;
            _maxScatter.MarkerFill = OxyColors.Red;
            _medianScatter = new ScatterSeries();
            _medianScatter.Title = "Median";
            _medianScatter.MarkerFill = OxyColors.Purple;
            _medianLine = new LineSeries();
            _medianLine.Color = OxyColors.Purple;

            this.DataPlotModel = new PlotModel();

            var model = new CellModel(InputGridView.SelectedCells[0]);
            if (model.IsColumnSectionId) { return; }
            if (IsYearPlot)
            {
                AddPlotInfo(model.Year.ToString(), "Section ID");
                foreach (var row in this.InputRows)
                {
                    var rowIndex = this.InputRows.IndexOf(row) + 1;
                    var entry = string.Empty;
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
                        AddPointsToPlot(entry, _inputScatter, inputCandleStick, Convert.ToDouble(rowIndex));
                    }
                }
            }
            else
            {
                AddPlotInfo(model.SectionId, "Year");
                var row = model.Row;
                foreach (var column in this.InputGridView.Columns)
                {
                    var year = column.Header.ToString();
                    var entry = row[year].ToString();

                    if (year != CellModel.SectionIdHeader && !String.IsNullOrEmpty(entry))
                    {
                        AddPointsToPlot(entry, _inputScatter, inputCandleStick, Convert.ToDouble(year));
                    }
                }
            }

            this.DataPlotModel.MouseDown += (s, e) =>
            {
                var scatter = _medianScatter;
                if (PlotLineType == LineType.Max)
                {
                    scatter = _maxScatter;
                }
                else if (PlotLineType == LineType.Min)
                {
                    scatter = _minScatter;
                }

                if (e.ChangedButton == OxyMouseButton.Left)
                {
                    var point = Axis.InverseTransform(e.Position, this.DataPlotModel.DefaultXAxis, this.DataPlotModel.DefaultYAxis);

                    if (DoesPointExist((int)point.X, scatter))
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
                    UpdateLine(_maxLine, scatter);
                }
                else if (PlotLineType == LineType.Min)
                {
                    UpdateLine(_minLine, scatter);
                }
                else
                {
                    UpdateLine(_medianLine, scatter);
                }
                this.DataPlotModel.InvalidatePlot(true);
            };


            this.DataPlotModel.Series.Add(_inputScatter);
            this.DataPlotModel.Series.Add(inputCandleStick);
            this.DataPlotModel.Series.Add(_minLine);
            this.DataPlotModel.Series.Add(_maxLine);
            this.DataPlotModel.Series.Add(_medianLine);
            this.DataPlotModel.Series.Add(_maxScatter);
            this.DataPlotModel.Series.Add(_minScatter);
            this.DataPlotModel.Series.Add(_medianScatter);

            this.DataPlotModel.Axes[0].IsZoomEnabled = false;
            this.DataPlotModel.Axes[0].IsPanEnabled = false;
            this.DataPlotModel.Axes[1].IsPanEnabled = false;

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

        private static bool DoesPointExist(int xCoord, ScatterSeries series)
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
    }
}
