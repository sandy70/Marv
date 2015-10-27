using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Marv.Common;
using Marv.Common.Types;
using Telerik.Charting;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ChartView;

namespace Marv.Input
{
    public static class Utils
    {
        public const string MaxInterpolatorLine = "MaximumLine";
        public const string MinInterpolatorLine = "MinimumLine";
        public const string ModeInterpolatorLine = "ModeLine";

        public static void AddTriangularDistributionAnnotation(this RadCartesianChart chart, Vertex selectedVertex, EvidenceRow dataRow, string columnName)
        {
            var from = (double) dataRow["From"];
            var to = (double) dataRow["To"];
            var vertexEvidence = dataRow[columnName] as VertexEvidence;

            var fill1 = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1)
            };

            fill1.GradientStops.Add(new GradientStop { Offset = 0, Color = Color.FromArgb(255, 218, 165, 32) });
            fill1.GradientStops.Add(new GradientStop { Offset = 1, Color = Color.FromArgb(50, 218, 165, 32) });

            chart.Annotations.Add(new CartesianMarkedZoneAnnotation
            {
                Fill = fill1,
                HorizontalFrom = @from,
                HorizontalTo = to,
                Stroke = new SolidColorBrush(Colors.Goldenrod),
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

            chart.Annotations.Add(new CartesianMarkedZoneAnnotation
            {
                Fill = fill2,
                HorizontalFrom = @from,
                HorizontalTo = to,
                Stroke = new SolidColorBrush(Colors.Goldenrod),
                Tag = dataRow,
                VerticalFrom = vertexEvidence.Params[2],
                VerticalTo = vertexEvidence.Params[1],
                ZIndex = -200
            });
        }

        public static void AddInterpolatedDistributionAnnotation(this RadCartesianChart chart, Vertex selectedVertex, EvidenceRow dataRow, string columnName)
        {
            var from = (double) dataRow["From"];
            var to = (double) dataRow["To"];
            var vertexEvidence = dataRow[columnName] as VertexEvidence;

            foreach (var state in selectedVertex.States)
            {
                var stateIndex = selectedVertex.States.IndexOf(state);
                var gammaAdjustedValue = Math.Pow(vertexEvidence.Value[stateIndex], 0.7);

                chart.Annotations.Add(new CartesianMarkedZoneAnnotation
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

        public static void AddLineAnnotation(this RadCartesianChart chart, EvidenceRow dataRow, string columnName)
        {
            var from = (double) dataRow["From"];
            var to = (double) dataRow["To"];
            var vertexEvidence = dataRow[columnName] as VertexEvidence;
            var verticalValue = vertexEvidence.Type == VertexEvidenceType.Number
                                    ? vertexEvidence.Params[0]
                                    : vertexEvidence.Params[vertexEvidence.Value.IndexOf(val => val == 1)];

            chart.Annotations.Add(new CartesianCustomLineAnnotation
            {
                HorizontalFrom = @from,
                HorizontalTo = to,
                Stroke = new SolidColorBrush(Colors.Goldenrod),
                StrokeThickness = 2,
                Tag = dataRow,
                VerticalFrom = verticalValue,
                VerticalTo = verticalValue,
                ZIndex = -200
            });
        }

        public static void AddNodeStateLines(this RadCartesianChart chart, Vertex selectedVertex, double baseTableMax, double baseTableMin)
        {
            var intervals = selectedVertex.States.Select(state => state.Min).Concat(selectedVertex.States.Last().Max.Yield()).ToArray();
            var newIntervals = new double[intervals.Count()];

            if (intervals.Any(val => val == Double.PositiveInfinity)) // workaround to replace infinity
            {
                Array.Sort(intervals);

                for (var i = 0; i < intervals.Count() - 1; i++)
                {
                    newIntervals[i] = intervals[i];
                }
                newIntervals[newIntervals.Count() - 1] = newIntervals[newIntervals.Count() - 2] * 10;
            }
            else
            {
                newIntervals = selectedVertex.GetIntervals().ToArray();
            }

            if (selectedVertex.Type == VertexType.Labelled)
            {
                newIntervals = Enumerable.Range(0, newIntervals.Count() + 1).Select(i => (double) i).ToArray();
            }
            foreach (var val in newIntervals)
            {
                chart.Annotations.Add(new CartesianCustomLineAnnotation
                {
                    HorizontalFrom = baseTableMin,
                    HorizontalTo = baseTableMax,
                    Stroke = new SolidColorBrush(Colors.Gray),
                    StrokeThickness = 1,
                    VerticalFrom = val,
                    VerticalTo = val,
                    ZIndex = 200
                });
            }
        }

        public static void AddPointAnnotation(this RadCartesianChart chart, EvidenceRow dataRow, string columnName)
        {
            var from = (double) dataRow["From"];
            var to = (double) dataRow["To"];
            var comment = (string) dataRow["Comment"];
            var vertexEvidence = dataRow[columnName] as VertexEvidence;
            var verticalValue = vertexEvidence.Type == VertexEvidenceType.Number
                                    ? vertexEvidence.Params[0]
                                    : vertexEvidence.Params[vertexEvidence.Value.IndexOf(val => val == 1)];

            var stackPanel = new StackPanel { Orientation = Orientation.Vertical };

            var commentTextBlock = new TextBlock { Text = comment };
            var ellipse = new Ellipse
            {
                Fill = new SolidColorBrush(Colors.Goldenrod),
                Height = 8,
                Stroke = new SolidColorBrush(Colors.Goldenrod),
                Width = 8,
            };

            stackPanel.Children.Add(commentTextBlock);
            stackPanel.Children.Add(ellipse);

            chart.Annotations.Add(new CartesianCustomAnnotation
            {
                Content = stackPanel,
                HorizontalAlignment = HorizontalAlignment.Center,
                HorizontalValue = (from + to) / 2,
                Tag = dataRow,
                VerticalAlignment = VerticalAlignment.Center,
                VerticalValue = verticalValue,
                ZIndex = -200
            });
        }

        public static void AddRangeAnnotation(this RadCartesianChart chart, EvidenceRow dataRow, string columnName)
        {
            var from = (double) dataRow["From"];
            var to = (double) dataRow["To"];

            var vertexEvidence = dataRow[columnName] as VertexEvidence;

            chart.Annotations.Add(new CartesianMarkedZoneAnnotation
            {
                Fill = new SolidColorBrush(Colors.Goldenrod),
                HorizontalFrom = @from,
                HorizontalTo = to,
                Stroke = new SolidColorBrush(Colors.Goldenrod),
                Tag = dataRow,
                VerticalFrom = vertexEvidence.Params[0],
                VerticalTo = vertexEvidence.Params[1],
                ZIndex = -200,
            });
        }

        public static List<double> CreateBaseRowsList(double baseMin, double baseMax, double baseRange)
        {
            var baseRowsList = new List<double>();

            for (var i = baseMin; i <= baseMax; i += baseRange)
            {
                baseRowsList.Add(i);
            }

            return baseRowsList;
        }

        public static double Distance(ScatterDataPoint p1, ScatterDataPoint p2)
        {
            if (p1.YValue != null && p2.YValue != null)
            {
                return Math.Sqrt(Math.Pow(p1.XValue - p2.XValue, 2) + Math.Pow((double) p1.YValue - (double) p2.YValue, 2));
            }

            return 0;
        }

        public static double Distance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        public static double GetInterpolatorPosition(this Vertex selectedVertex, string line)
        {
            double linearPosition = 0;
            double logarithmicPosition = 0;

            if (line == ModeInterpolatorLine)
            {
                linearPosition = (selectedVertex.SafeMax + selectedVertex.SafeMin) / 2;

                logarithmicPosition = Math.Pow(10,
                    selectedVertex.SafeMin == 0
                        ? Math.Log10(selectedVertex.SafeMax) / 2
                        : (Math.Log10(selectedVertex.SafeMax) + Math.Log10(selectedVertex.SafeMin)) / 2);
            }

            if (line == MaxInterpolatorLine)
            {
                linearPosition = (selectedVertex.SafeMax + (selectedVertex.SafeMin + selectedVertex.SafeMax) / 2) / 2;
                logarithmicPosition = Math.Pow(10,
                    selectedVertex.SafeMin == 0
                        ? 0.75 * Math.Log10(selectedVertex.SafeMax)
                        : (Math.Log10(selectedVertex.SafeMax) + (Math.Log10(selectedVertex.SafeMax) + Math.Log10(selectedVertex.SafeMin)) / 2) / 2);
            }

            if (line == MinInterpolatorLine)
            {
                linearPosition = (selectedVertex.SafeMin + (selectedVertex.SafeMin + selectedVertex.SafeMax) / 2) / 2;
                logarithmicPosition = Math.Pow(10,
                    selectedVertex.SafeMin == 0
                        ? 0.25 * Math.Log10(selectedVertex.SafeMax)
                        : (Math.Log10(selectedVertex.SafeMin) + (Math.Log10(selectedVertex.SafeMax) + Math.Log10(selectedVertex.SafeMin)) / 2) / 2);
            }

            return selectedVertex.AxisType == VertexAxisType.Linear ? linearPosition : logarithmicPosition;
        }

        public static Point GetPointOnChart(this RadCartesianChart chart, ScatterDataPoint scatterPoint)
        {
            return chart.ConvertDataToPoint(new DataTuple(scatterPoint.XValue, scatterPoint.YValue));
        }

        public static IEnumerable<double> GetXCoords(this ObservableCollection<ScatterDataPoint> numberPoints)
        {
            return numberPoints.Select(scatterDataPoint => scatterDataPoint.XValue);
        }

        public static IEnumerable<double> GetYCoords(this ObservableCollection<ScatterDataPoint> numberPoints)
        {
            return numberPoints.Select(scatterDataPoint => scatterDataPoint.YValue != null ? scatterDataPoint.YValue.Value : 0);
        }

        public static Dict<string, EvidenceTable> Merge(Dict<string, NodeData> userDataObj, List<double> baseRowsList, Network network)
        {
            var unmergedEvidenceSet = new Dict<string, EvidenceTable>();
            var mergedEvidenceSet = new Dict<string, EvidenceTable>();
            var newList = new List<double>();

            if (baseRowsList != null)
            {
                newList = baseRowsList.ToList();
            }

            foreach (var kvp in userDataObj)
            {
                foreach (var row in kvp.Value.UserTable)
                {
                    var colNames = row.GetDynamicMemberNames().ToList();

                    foreach (var colName in colNames)
                    {
                        if (row[colName] == "")
                        {
                            row[colName] = new VertexEvidence { Type = VertexEvidenceType.Null };
                        }
                    }
                }
            }

            foreach (var kvp in userDataObj)
            {
                unmergedEvidenceSet.Add(kvp.Key, kvp.Value.UserTable);
            }

           

            // Generate a list which holds the modified section ranges
            foreach (var kvp in unmergedEvidenceSet)
            {
                var evidenceTable = kvp.Value;

                foreach (var evidenceRow in evidenceTable)
                {
                    newList.Add(evidenceRow.From);
                    newList.Add(evidenceRow.To);
                }
            }

            newList.Sort(); // sorting the new list

            foreach (var kvp in unmergedEvidenceSet)
            {
                var unmergeEvidenceTableKey = kvp.Key;
                var unmergedEvidenceTable = kvp.Value;

                if (unmergedEvidenceTable.Count == 0)
                {
                    continue;
                }

                var mergedEvidenceTable = new EvidenceTable(unmergedEvidenceTable.DateTimes);

                for (var i = 0; i < newList.Count - 1; i++)
                {
                    var evidenceRow = new EvidenceRow { From = newList[i], To = newList[i + 1] };

                    // If this is a point feature and no table contains it, then don't add.
                    if (!unmergedEvidenceSet.Contains(evidenceRow) && newList[i] == newList[i + 1])
                    {
                        continue;
                    }

                    // multiple inputs for a single point feature are allowed 
                    if (unmergedEvidenceSet.Contains(evidenceRow) && newList[i] == newList[i + 1])
                    {
                        mergedEvidenceTable.Add(evidenceRow);
                    }
                    else
                    {
                        mergedEvidenceTable.AddUnique(evidenceRow);
                    }
                }

                EvidenceRow uniqueRow = null;
                var deleteRows = new ObservableCollection<EvidenceRow>();

                foreach (var mergedEvidenceRow in mergedEvidenceTable)
                {
                    if (uniqueRow == null)
                    {
                        uniqueRow = mergedEvidenceRow;
                        continue;
                    }

                    if (mergedEvidenceRow.From ==uniqueRow.From && mergedEvidenceRow.To == uniqueRow.To)
                    {
                        deleteRows.Add(mergedEvidenceRow);
                    }
                    else
                    {
                        uniqueRow = mergedEvidenceRow;
                    }
                }

                mergedEvidenceTable.Remove(deleteRows);

                foreach (var mergedEvidenceRow in mergedEvidenceTable)
                {
                    var unmergedEvidenceRows = unmergedEvidenceTable.Where(row => row.Contains(mergedEvidenceRow)).ToList();
                    var columnNames = mergedEvidenceRow.GetDynamicMemberNames().ToList();

                    if (unmergedEvidenceRows.Count() > 1) // merged section has multiple input values 
                    {
                        var avgEvidenceValues = GetEvidenceAverage(unmergedEvidenceRows);

                        foreach (var columnName in columnNames)
                        {
                            var evidenceString = avgEvidenceValues[columnName].ValueToDistribution();
                            mergedEvidenceRow[columnName] = network.Vertices[unmergeEvidenceTableKey].States.ParseEvidenceString(evidenceString);
                        }
                    }

                    else
                    {
                        foreach (var columnName in columnNames)
                        {
                            mergedEvidenceRow[columnName] = !unmergedEvidenceRows.Any()
                                                                ? new VertexEvidence { Type = VertexEvidenceType.Null }
                                                                : unmergedEvidenceRows.FirstOrDefault()[columnName];
                        }
                    }
                }

                mergedEvidenceSet.Add(unmergeEvidenceTableKey, mergedEvidenceTable);
            }

            return mergedEvidenceSet;
        }

        public static void UpdateCommentBlocks(this RadCartesianChart chart, EvidenceRow row, NumericalAxis verticalAxis)
        {
            var fillBrush = new SolidColorBrush(Color.FromRgb(236, 236, 236));
            var strokeBrush = new SolidColorBrush(Colors.LightGray);

            var from = (double) row["From"];
            var to = (double) row["To"];
            var comment = (string) row["Comment"];

            chart.Annotations.Remove(annotation => ReferenceEquals(annotation.Tag, row));

            if (from == to)
            {
                chart.Annotations.Add(new CartesianCustomLineAnnotation
                {
                    HorizontalFrom = @from,
                    HorizontalTo = to,
                    Stroke = strokeBrush,
                    StrokeThickness = 2,
                    Tag = row,
                    VerticalFrom = verticalAxis.Minimum,
                    VerticalTo = verticalAxis.Maximum,
                    ZIndex = -500
                });
            }

            else
            {
                chart.Annotations.Add(new CartesianMarkedZoneAnnotation
                {
                    Fill = fillBrush,
                    HorizontalFrom = @from,
                    HorizontalTo = to,
                    Stroke = strokeBrush,
                    Tag = row,
                    VerticalFrom = verticalAxis.Minimum,
                    VerticalTo = verticalAxis.Maximum,
                    ZIndex = -500
                });
            }
        }

        public static Dict<string, EvidenceTable> UpdateWithInterpolatedData(this Dict<string, EvidenceTable> mergedEvidenceSet, Dict<string, EvidenceTable> interpolatedDataSet)
        {
            if (interpolatedDataSet == null)
            {
                return mergedEvidenceSet;
            }

            foreach (var kvp in interpolatedDataSet)
            {
                var nodeKey = kvp.Key;

                foreach (var mergedEvidenceRow in mergedEvidenceSet[nodeKey])
                {
                    foreach (var interpolatedRow in kvp.Value)
                    {
                        if (!interpolatedRow.Contains(mergedEvidenceRow))
                        {
                            continue;
                        }
                        var columnNames = mergedEvidenceRow.GetDynamicMemberNames().ToList();

                        foreach (var columnName in columnNames)
                        {
                            var vertexEvidence = interpolatedRow[columnName] as VertexEvidence;
                            if (vertexEvidence == null)
                            {
                                continue;
                            }
                            mergedEvidenceRow[columnName] = vertexEvidence;
                        }
                    }
                }
            }

            return mergedEvidenceSet;
        }

        public static string ValueToDistribution(this double[] evidenceValue)
        {
            var evidenceString = "";
            var i = 0;
            while (i < evidenceValue.Length)
            {
                evidenceString += evidenceValue[i] + ",";
                i++;
            }
            evidenceString = evidenceString.Substring(0, evidenceString.Length - 1);

            return evidenceString;
        }

        private static bool Contains(this Dict<string, EvidenceTable> evidenceSet, EvidenceRow evidenceRow)
        {
            var values = new ObservableCollection<EvidenceTable>();

            foreach (var kvp in evidenceSet)
            {
                values.Add(kvp.Value);
            }

            return values.Any(table => table.Any(row => row.From==evidenceRow.From && row.To == evidenceRow.To));
        }

        private static Dict<string, double[]> GetEvidenceAverage(List<EvidenceRow> unmergedEvidenceRows)
        {
            var columnNames = unmergedEvidenceRows[0].GetDynamicMemberNames().ToList();
            var noOfstates=0;
           
            foreach (var row in unmergedEvidenceRows)
            {
                foreach (var colName in columnNames)
                {
                    if ((row[colName] as VertexEvidence).Type != VertexEvidenceType.Null)
                    {
                        noOfstates = (row[colName] as VertexEvidence).Value.Count();
                        goto loopExited;
                    }
                    
                }
            }

            loopExited:
            var combinedColumnValues = new Dict<string, double[]>();

            foreach (var columnName in columnNames)
            {
                combinedColumnValues.Add(columnName, new double[noOfstates]);

                foreach (var evidenceRow in unmergedEvidenceRows)
                {
                    var i = 0;
                    var evidence = evidenceRow[columnName] as VertexEvidence;
                    
                    if (evidence.Value == null)
                    {
                        continue;
                    }
                    while (i < noOfstates)
                    {
                        combinedColumnValues[columnName][i] += evidence.Value[i];
                        i++;
                    }
                }
            }

            foreach (var kvp in combinedColumnValues)
            {
                var colValue = kvp.Value;

                for (var i = 0; i < colValue.Length; i++)
                {
                    colValue[i] = colValue[i] / unmergedEvidenceRows.Count;
                }
            }

            return combinedColumnValues;
        }
    }
}