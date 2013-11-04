﻿using Marv.Common;
using Marv.Controls;
using NLog;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media;
using Telerik.Charting;
using Telerik.Windows.Controls.ChartView;

namespace Marv
{
    internal class MainWindowGraphControlBehavior : Behavior<MainWindow>
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            var window = this.AssociatedObject;

            window.GraphControl.StateDoubleClicked += GraphControl_StateDoubleClicked;

            MainWindow.VertexChartCommand.Executed += VertexChartCommand_Executed;
            MainWindow.VertexChartPofCommand.Executed += VertexChartPofCommand_Executed;
            MainWindow.VertexBarChartCommand.Executed += VertexBarChartCommand_Executed;

            VertexCommand.VertexClearCommand.Executed += VertexClearCommand_Executed;
            VertexCommand.VertexLockCommand.Executed += VertexLockCommand_Executed;
            VertexCommand.VertexSubGraphCommand.Executed += VertexSubGraphCommand_Executed;
        }

        private void VertexChartPofCommand_Executed(object sender, Vertex e)
        {
            var window = this.AssociatedObject;
            window.IsChartControlVisible = true;

            var chartSeries = new ChartSeries<ScatterDataPoint>
            {
                Name = "Prob. of Failure",
                Type = typeof(ScatterLineSeries)
            };

            foreach (var year in window.graphValueTimeSeries.Keys)
            {
                var pof = window.graphValueTimeSeries[year]["coatd"]["YEs"];

                chartSeries.Add(new ScatterDataPoint
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

        private void VertexBarChartCommand_Executed(object sender, Vertex vertex)
        {
            var window = this.AssociatedObject;
            window.IsChartControlVisible = true;

            var categoryPoints = new ChartSeries<CategoricalDataPoint>
            {
                Name = vertex.Name,
                Type = typeof(Telerik.Windows.Controls.ChartView.BarSeries),
            };

            foreach (var state in vertex.States)
            {
                var value = window.graphValueTimeSeries[1973][vertex.Key][state.Key];

                categoryPoints.Add(new CategoricalDataPoint
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

            var colorForYear = new Dict<int, Color>
            {
                { 1973, Colors.Red },
                { 1993, Colors.Green },
                { 2004, Colors.Blue },
                { 2011, Colors.Orange }
            };

            foreach (var year in colorForYear.Keys)
            {
                var chartSeries = new ChartSeries<ScatterDataPoint>
                {
                    Name = year.ToString(),
                    Stroke = new SolidColorBrush(colorForYear[year]),
                    Type = typeof(ScatterSplineSeries),
                };

                foreach (var state in vertex.States)
                {
                    var x = (state.Range.Min + state.Range.Max) / 2;
                    var y = window.graphValueTimeSeries[year][vertex.Key][state.Key];

                    chartSeries.Add(new ScatterDataPoint
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

        private void GraphControl_StateDoubleClicked(object sender, BnGraphControlEventArgs e)
        {
            var window = this.AssociatedObject;
            var graph = window.SourceGraph;
            var vertex = e.Vertex;

            if (e.Vertex.SelectedState != e.State)
            {
                var evidence = new HardEvidence
                {
                    StateIndex = vertex.States.IndexOf(e.State)
                };

                try
                {
                    graph.Value = graph.Run(vertex.Key, evidence);
                }
                catch (Smile.SmileException)
                {
                    window.Notifications.Push(new NotificationTimed
                    {
                        Name = "Inconsistent Evidence",
                        Description = "Inconsistent evidence entered for vertex: " + vertex.Name,
                    });

                    graph.Value = graph.ClearEvidence(vertex.Key);
                }
            }
            else
            {
                graph.Value = graph.ClearEvidence(vertex.Key);
            }
        }

        private void VertexClearCommand_Executed(object sender, Vertex vertex)
        {
            var graph = this.AssociatedObject.SourceGraph;
            graph.Value = graph.ClearEvidence(vertex.Key);
        }

        private void VertexLockCommand_Executed(object sender, Vertex vertex)
        {
            var window = this.AssociatedObject;
            var graph = window.SourceGraph;

            try
            {
                if (vertex.IsLocked)
                {
                    graph.Value = graph.Run(vertex.Key, vertex.ToEvidence());
                }
            }
            catch (Smile.SmileException)
            {
                window.Notifications.Push(new NotificationTimed
                {
                    Name = "Inconsistent Evidence",
                    Description = "Inconsistent evidence entered for vertex: " + vertex.Name,
                });

                graph.Value = graph.ClearEvidence(vertex.Key);
            }
        }

        private void VertexSubGraphCommand_Executed(object sender, Vertex vertex)
        {
            var window = this.AssociatedObject;
            var displayGraph = window.DisplayGraph;
            var sourceGraph = window.SourceGraph;

            window.DisplayGraph = sourceGraph.GetSubGraph(vertex.HeaderOfGroup);
            window.IsBackButtonVisible = true;
        }
    }
}