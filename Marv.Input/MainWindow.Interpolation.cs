using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Marv.Common;
using Telerik.Charting;
using Telerik.Windows.Controls.ChartView;

namespace Marv.Input
{
    public partial class MainWindow
    {
        private ScatterDataPoint capturedPoint;

        private void InterpolationDataClearButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.SelectedInterpolationData == null)
            {
                return;
            }
            this.SelectedInterpolationData.Points = null;
            this.SelectedInterpolationData.Type = null;
            this.CopiedColumnName = null;
        }

        private void InterpolationSeries_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var series = sender as ScatterLineSeries;

            var points = series.DataContext as ObservableCollection<ScatterDataPoint>;

            var mousePosition = e.MouseDevice.GetPosition(this.Chart);

            var data = this.Chart.ConvertPointToData(mousePosition);

            var mousePoint = new ScatterDataPoint
            {
                XValue = (double) data.FirstValue,
                YValue = (double) data.SecondValue
            };

            var nextPoint = points.First(point => point.XValue > mousePoint.XValue);

            var index = points.IndexOf(nextPoint);

            points.Insert(index, mousePoint);
        }

        private void InterpolationSeries_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var series = sender as ScatterLineSeries;

            series.CaptureMouse();

            var points = series.DataContext as ObservableCollection<ScatterDataPoint>;

            var mousePosition = e.MouseDevice.GetPosition(this.Chart);

            var data = this.Chart.ConvertPointToData(mousePosition);

            if (data.FirstValue == null || data.SecondValue == null)
            {
                return;
            }

            var nearestPoint = points.MinBy(p => Utils.Distance(p,
                new ScatterDataPoint
                {
                    XValue = (double) data.FirstValue,
                    YValue = (double) data.SecondValue
                }));

            var nearestPointPosition = this.Chart.ConvertDataToPoint(new DataTuple(nearestPoint.XValue, nearestPoint.YValue));

            this.capturedPoint = Utils.Distance(mousePosition, nearestPointPosition) < 10 ? nearestPoint : null;
        }

        private void InterpolationSeries_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                return;
            }

            var data = this.Chart.ConvertPointToData(e.MouseDevice.GetPosition(this.Chart));

            if (data.FirstValue == null || data.SecondValue == null)
            {
                return;
            }

            var mousePoint = new ScatterDataPoint
            {
                XValue = (double) data.FirstValue,
                YValue = (double) data.SecondValue
            };

            if (this.capturedPoint != null)
            {
                var newPoint = new ScatterDataPoint
                {
                    XValue = this.capturedPoint.XValue,
                    YValue = mousePoint.YValue
                };

                ((sender as ScatterLineSeries).DataContext as ObservableCollection<ScatterDataPoint>).Replace(this.capturedPoint, newPoint);

                this.capturedPoint = newPoint;
            }
        }

        private void InterpolationSeries_MouseRightDown(object sender, MouseButtonEventArgs e)
        {
            var series = sender as ScatterLineSeries;

            var points = series.DataContext as ObservableCollection<ScatterDataPoint>;

            var mousePosition = e.MouseDevice.GetPosition(this.Chart);

            var closestPointDistance = Utils.Distance(this.Chart.GetPointOnChart(points[0]), mousePosition);

            ScatterDataPoint closestScatterPoint = null;
            foreach (var scatterDataPoint in points)
            {
                var linePoint = this.Chart.GetPointOnChart(scatterDataPoint);

                closestPointDistance = Math.Min(closestPointDistance, Utils.Distance(linePoint, mousePosition));

                if (!(closestPointDistance < Tolerance))
                {
                    continue;
                }
                closestScatterPoint = scatterDataPoint;
                break;
            }

            if (!(closestPointDistance < Tolerance))
            {
                return;
            }

            points.Remove(closestScatterPoint);
        }

        private void InterpolationSeries_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var series = sender as ScatterLineSeries;

            series.ReleaseMouseCapture();

            this.capturedPoint = null;
        }
    }
}