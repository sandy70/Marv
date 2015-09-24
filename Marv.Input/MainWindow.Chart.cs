using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Marv.Common;
using Telerik.Windows.Controls.ChartView;

namespace Marv.Input
{
    public partial class MainWindow
    {
        private void Plot(string columnName)
        {
            this.Chart.AddNodeStateLines(this.SelectedVertex, this.BaseTableMax, this.BaseTableMin);

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
                this.Chart.Annotations.Remove(annotation => ReferenceEquals(annotation.Tag, dataRow));
                return;
            }

            // Remove older annotations

            this.Chart.Annotations.Remove(annotation => ReferenceEquals(annotation.Tag, dataRow));

            this.Chart.AddNodeStateLines(this.SelectedVertex, this.BaseTableMax, this.BaseTableMin);

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
                if (this.SelectedTheme == DataTheme.User)
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

                else
                {
                    foreach (var state in this.SelectedVertex.States)
                    {
                        var stateIndex = this.SelectedVertex.States.IndexOf(state);
                        var gammaAdjustedValue = Math.Pow(vertexEvidence.Value[stateIndex], 0.7);

                        this.Chart.Annotations.Add(new CartesianMarkedZoneAnnotation
                        {
                            Fill = new SolidColorBrush(Color.FromArgb((byte) (gammaAdjustedValue * 255), 218, 165, 32)),
                            HorizontalFrom = @from,
                            HorizontalTo = to,
                            Stroke = new SolidColorBrush(Color.FromArgb((byte) (gammaAdjustedValue * 255), 218, 165, 32)),
                            Tag = dataRow,
                            VerticalFrom = state.SafeMin,
                            VerticalTo = state.SafeMax,
                            ZIndex = -200
                        });
                    }
                }
            }
            else if (vertexEvidence.Type != VertexEvidenceType.Null && vertexEvidence.Type != VertexEvidenceType.State)
            {
                if (this.SelectedTheme != DataTheme.User)
                {
                    foreach (var state in this.SelectedVertex.States)
                    {
                        var stateIndex = this.SelectedVertex.States.IndexOf(state);
                        var gammaAdjustedValue = Math.Pow(vertexEvidence.Value[stateIndex], 0.7);

                        this.Chart.Annotations.Add(new CartesianMarkedZoneAnnotation
                        {
                            Fill = new SolidColorBrush(Color.FromArgb((byte) (gammaAdjustedValue * 255), 218, 165, 32)),
                            HorizontalFrom = @from,
                            HorizontalTo = to,
                            Stroke = new SolidColorBrush(Color.FromArgb((byte) (gammaAdjustedValue * 255), 218, 165, 32)),
                            Tag = dataRow,
                            VerticalFrom = state.SafeMin,
                            VerticalTo = state.SafeMax,
                            ZIndex = -200
                        });
                    }
                }

                else
                {
                    var fill1 = new LinearGradientBrush
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(0, 1)
                    };

                    fill1.GradientStops.Add(new GradientStop { Offset = 0, Color = Color.FromArgb(255, 218, 165, 32) });
                    fill1.GradientStops.Add(new GradientStop { Offset = 1, Color = Color.FromArgb(0, 218, 165, 32) });

                    this.Chart.Annotations.Add(new CartesianMarkedZoneAnnotation
                    {
                        Fill = fill1,
                        HorizontalFrom = @from,
                        HorizontalTo = to,
                        Stroke = strokeBrush,
                        Tag = dataRow,
                        VerticalFrom = vertexEvidence.Params[1],
                        VerticalTo = vertexEvidence.Params[0],
                        ZIndex = -200
                    });

                    var fill2 = new LinearGradientBrush
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(0, 1)
                    };

                    fill2.GradientStops.Add(new GradientStop { Offset = 0, Color = Color.FromArgb(0, 218, 165, 32) });
                    fill2.GradientStops.Add(new GradientStop { Offset = 1, Color = Color.FromArgb(255, 218, 165, 32) });

                    this.Chart.Annotations.Add(new CartesianMarkedZoneAnnotation
                    {
                        Fill = fill2,
                        HorizontalFrom = @from,
                        HorizontalTo = to,
                        Stroke = strokeBrush,
                        Tag = dataRow,
                        VerticalFrom = vertexEvidence.Params[2],
                        VerticalTo = vertexEvidence.Params[1],
                        ZIndex = -200
                    });
                }
            }

            else if (vertexEvidence.Type == VertexEvidenceType.State)
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
                        VerticalValue = vertexEvidence.Params[vertexEvidence.Value.IndexOf(val => val == 1)],
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
                        StrokeThickness = 4,
                        Tag = dataRow,
                        VerticalFrom = vertexEvidence.Params[vertexEvidence.Value.IndexOf(val => val == 1)],
                        VerticalTo = vertexEvidence.Params[vertexEvidence.Value.IndexOf(val => val == 1)],
                        ZIndex = -200
                    });
                }
            }
        }
    }
}