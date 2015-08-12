using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Marv.Common;
using Telerik.Charting;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ChartView;

namespace Marv.Input
{
    public partial class MainWindow
    {
        private void Chart_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.draggedPoint != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var chart = (RadCartesianChart) sender;

                var data = chart.ConvertPointToData(e.GetPosition(chart));

                this.IsLineCross = !this.CurrentInterpolatorDataPoints.IsWithInRange();

                this.DraggedPoint.YValue = (double) (data.SecondValue);
                ScatterDataPoint replacePoint = null;

                this.CurrentInterpolatorDataPoints = this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName];

                var currentLine = this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].GetNumberPoints(this.SelectedLine);

                foreach (var userPoint in currentLine)
                {
                    if (userPoint.XValue.Equals(this.DraggedPoint.XValue))
                    {
                        replacePoint = userPoint;
                    }
                }

                this.CurrentInterpolatorDataPoints.GetNumberPoints(this.SelectedLine).Replace(replacePoint, this.DraggedPoint);
                this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].GetNumberPoints(this.SelectedLine).Replace(replacePoint, this.DraggedPoint);
            }

            else if (this.draggedPoint == null && e.LeftButton == MouseButtonState.Pressed && this.IsInterpolateClicked)
            {
                var chart = (RadCartesianChart) sender;

                var dynamicPoint = e.GetPosition(chart);

                this.CurrentInterpolatorDataPoints = this.UserNumberPoints[this.SelectedVertex.Key][this.SelectedColumnName];

                var currentLine = this.CurrentInterpolatorDataPoints.GetNumberPoints(this.SelectedLine);

                ScatterDataPoint replacePoint = null;

                foreach (var scatterPoint in currentLine)
                {
                    var linePoint = this.Chart.GetPointOnChart(scatterPoint);

                    if (Math.Round(linePoint.X)==Math.Round(dynamicPoint.X) && Math.Abs(linePoint.Y - dynamicPoint.Y) < ModifyTolerance)
                    {
                        replacePoint = scatterPoint;
                    }
                }

                this.IsLineCross = !this.CurrentInterpolatorDataPoints.IsWithInRange();
               
                if (!this.IsLineCross)
                {
                    currentLine.Replace(replacePoint, this.Chart.GetScatterDataPoint(dynamicPoint));
                }
            }
        }

        private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.draggedPoint = ((sender as Ellipse).DataContext as ScatterDataPoint);
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

            this.Chart.Annotations.Remove(annotation => ReferenceEquals(annotation.Tag, dataRow));

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
            else if (vertexEvidence.Type != VertexEvidenceType.Null && vertexEvidence.Type != VertexEvidenceType.State)
            {
                var maxValue = vertexEvidence.Value.Max();

                var fill = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(0, 1)
                };

                for (var i = vertexEvidence.Value.Count(); i > 0; i--)
                {
                    var value = vertexEvidence.Value[i - 1];

                    fill.GradientStops.Add(new GradientStop
                    {
                        Offset = 1 - (this.SelectedVertex.Intervals.ElementAt(i) - this.selectedVertex.SafeMin) / (this.SelectedVertex.SafeMax - this.selectedVertex.SafeMin),
                        Color = Color.FromArgb((byte) (value / maxValue * 255), 218, 165, 32)
                    });

                    fill.GradientStops.Add(new GradientStop
                    {
                        Offset = 1 - (this.SelectedVertex.Intervals.ElementAt(i - 1) - this.selectedVertex.SafeMin) / (this.SelectedVertex.SafeMax - this.selectedVertex.SafeMin),
                        Color = Color.FromArgb((byte) (value / maxValue * 255), 218, 165, 32)
                    });
                }

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
    }
}