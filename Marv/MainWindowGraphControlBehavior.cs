using System.Collections.Generic;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media;
using Marv.Common;
using Marv.Common.Graph;
using Marv.Controls;
using Telerik.Windows.Controls.ChartView;

namespace Marv
{
    internal class MainWindowGraphControlBehavior : Behavior<MainWindow>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow.VertexChartCommand.Executed += VertexChartCommand_Executed;
            MainWindow.VertexChartPofCommand.Executed += VertexChartPofCommand_Executed;
            MainWindow.VertexBarChartCommand.Executed += VertexBarChartCommand_Executed;

            VertexCommand.VertexClearCommand.Executed += VertexClearCommand_Executed;
        }

        private void VertexBarChartCommand_Executed(object sender, Vertex vertex)
        {
            var window = this.AssociatedObject;
            window.IsChartControlVisible = true;

            var categoryPoints = new ChartSeries<CategoricalPoint>
            {
                Name = vertex.Name,
                Type = typeof (BarSeries),
            };

            foreach (var state in vertex.States)
            {
                var value = window.GraphValues[1973][vertex.Key][state.Key];

                categoryPoints.Add(new CategoricalPoint
                {
                    Category = state.Key,
                    Value = value
                });
            }

            window.ChartSeries.Clear();
            window.ChartSeries.Add(categoryPoints);

            ChartAxes.VerticalCategoricalAxis.Title = "Probability";
        }

        private void VertexChartCommand_Executed(object sender, Vertex vertex)
        {
            var window = this.AssociatedObject;
            window.IsChartControlVisible = true;

            window.ChartSeries.Clear();

            var colorForYear = new Dictionary<int, Color>
            {
                {1973, Colors.Red},
                {1993, Colors.Green},
                {2004, Colors.Blue},
                {2011, Colors.Orange}
            };

            foreach (var year in colorForYear.Keys)
            {
                var chartSeries = new ChartSeries<ScatterPoint>
                {
                    Name = year.ToString(),
                    Stroke = new SolidColorBrush(colorForYear[year]),
                    Type = typeof (ScatterSplineSeries),
                };

                foreach (var state in vertex.States)
                {
                    var x = (state.Min + state.Max)/2;
                    var y = window.GraphValues[year][vertex.Key][state.Key];

                    chartSeries.Add(new ScatterPoint
                    {
                        XValue = x,
                        YValue = y,
                    });
                }

                window.ChartSeries.Add(chartSeries);
            }

            ChartAxes.HorizontalLinearAxis.Title = vertex.Name;
            ChartAxes.VerticalLinearAxis.Title = "Probability";
        }

        private void VertexChartPofCommand_Executed(object sender, Vertex e)
        {
            var window = this.AssociatedObject;
            window.IsChartControlVisible = true;

            var chartSeries = new ChartSeries<ScatterPoint>
            {
                Name = "Prob. of Failure",
                Type = typeof (ScatterLineSeries)
            };

            foreach (var year in window.GraphValues.Keys)
            {
                var pof = window.GraphValues[year]["coatd"]["YEs"];

                chartSeries.Add(new ScatterPoint
                {
                    XValue = year,
                    YValue = pof
                });
            }

            window.ChartSeries.Clear();
            window.ChartSeries.Add(chartSeries);

            ChartAxes.HorizontalLinearAxis.Title = "Time";
            ChartAxes.VerticalLinearAxis.Title = "Probability of Failure";
        }

        private void VertexClearCommand_Executed(object sender, Vertex vertex)
        {
            var graph = this.AssociatedObject.Graph;
            graph.Belief = graph.ClearEvidence(vertex.Key);
        }
    }
}