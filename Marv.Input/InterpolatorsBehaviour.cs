using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using Marv.Common;
using Marv.Controls;
using Telerik.Charting;
using Telerik.Windows.Controls.ChartView;

namespace Marv.Input
{
    internal class InterpolatorsBehaviour : Behavior<ScatterLineSeries>
    {
        private const double Tolerance = 5;
        private MainWindow mainWindow;

        protected override void OnAttached()
        {
            base.OnAttached();

            //Hook up event handlers
            this.AssociatedObject.MouseLeftButtonDown += AssociatedObject_MouseLeftButtonDown;
            this.AssociatedObject.MouseRightButtonDown += AssociatedObject_MouseRightButtonDown;
            this.AssociatedObject.MouseDoubleClick += AssociatedObject_MouseDoubleClick;

            mainWindow = this.AssociatedObject.GetParent<MainWindow>();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.AssociatedObject.MouseLeftButtonDown -= AssociatedObject_MouseLeftButtonDown;
            this.AssociatedObject.MouseRightButtonDown -= AssociatedObject_MouseRightButtonDown;
            this.AssociatedObject.MouseDoubleClick -= AssociatedObject_MouseDoubleClick;
        }

        private void AssociatedObject_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            mainWindow.SelectedLine = this.AssociatedObject.Name;

            var mousePosition = e.GetPosition(mainWindow.Chart);
            var userDataPoint = mainWindow.Chart.GetScatterDataPoint(mousePosition);

            InsertDataPoint(userDataPoint);
        }

        private void AssociatedObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mainWindow.SelectedLine = this.AssociatedObject.Name;

           
        }

        private void AssociatedObject_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            mainWindow.SelectedLine = this.AssociatedObject.Name;

            var selectedPoint = e.GetPosition(mainWindow.Chart);

            DeleteScatterDataPoint(selectedPoint);
        }

        private void DeleteScatterDataPoint(Point deletePoint)
        {
            var currentLine = mainWindow.UserNumberPoints[mainWindow.SelectedVertex.Key][mainWindow.SelectedColumnName].GetNumberPoints(mainWindow.SelectedLine);

            mainWindow.CurrentInterpolatorDataPoints = mainWindow.UserNumberPoints[mainWindow.SelectedVertex.Key][mainWindow.SelectedColumnName];

            var firstPoint = mainWindow.Chart.GetPointOnChart(currentLine[0]);

            var closestPointDistance = Utils.Distance(firstPoint, deletePoint);

            ScatterDataPoint closestScatterPoint = null;
            foreach (var scatterDataPoint in currentLine)
            {
                var linePoint = mainWindow.Chart.GetPointOnChart(scatterDataPoint);

                closestPointDistance = Math.Min(closestPointDistance, Utils.Distance(linePoint, deletePoint));

                if (closestPointDistance < Tolerance)
                {
                    closestScatterPoint = scatterDataPoint;
                    break;
                }
            }

            if (!(closestPointDistance < Tolerance))
            {
                return;
            }

            if (mainWindow.UserNumberPoints[mainWindow.SelectedVertex.Key] != null)
            {
                mainWindow.UserNumberPoints[mainWindow.SelectedVertex.Key][mainWindow.SelectedColumnName].GetNumberPoints(mainWindow.SelectedLine).Remove(closestScatterPoint);
            }
        }

        private void InsertDataPoint(ScatterDataPoint userDataPoint)
        {
            mainWindow.CurrentInterpolatorDataPoints = mainWindow.UserNumberPoints[mainWindow.SelectedVertex.Key][mainWindow.SelectedColumnName];

            var currentLine = mainWindow.CurrentInterpolatorDataPoints.GetNumberPoints(mainWindow.SelectedLine);

            if (currentLine != null)
            {
                var index = currentLine.GetXCoords().IndexOf(xcoord => xcoord > userDataPoint.XValue);
                currentLine.Insert(index, userDataPoint);
            }
        }
    }
}