using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Marv.Common;
using Marv.Common.Interpolators;
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
                logarithmicPosition = Math.Pow(10, selectedVertex.SafeMin == 0 ? 0.75 * Math.Log10(selectedVertex.SafeMax) : 
                                                                                (Math.Log10(selectedVertex.SafeMax) + (Math.Log10(selectedVertex.SafeMax) + Math.Log10(selectedVertex.SafeMin)) / 2) / 2);
            }

            if (line == MinInterpolatorLine)
            {
                linearPosition = (selectedVertex.SafeMin + (selectedVertex.SafeMin + selectedVertex.SafeMax) / 2) / 2;
                logarithmicPosition = Math.Pow(10, selectedVertex.SafeMin == 0 ? 0.25 * Math.Log10(selectedVertex.SafeMax) :
                                                                                (Math.Log10(selectedVertex.SafeMin) + (Math.Log10(selectedVertex.SafeMax) + Math.Log10(selectedVertex.SafeMin)) / 2) / 2);
            }

            return selectedVertex.AxisType == VertexAxisType.Linear ? linearPosition : logarithmicPosition;
        }

        public static Dict<string, double> GetMinMaxUserValues(this EvidenceTable userTable, string selectedColumnName)
        {
            var minUserValue = double.MaxValue;
            var maxUserValue = double.MinValue;

            var minMaxUserValues = new Dict<string, double>();

            foreach (var row in userTable)
            {
                var evidence = row[selectedColumnName] as VertexEvidence;

                if (evidence == null)
                {
                    continue;
                }

                maxUserValue = Math.Max(maxUserValue, evidence.Params.Max());
                minUserValue = Math.Min(minUserValue, evidence.Params.Min());

                minMaxUserValues.Add("Maximum", maxUserValue);
                minMaxUserValues.Add("Minimum", minUserValue);
            }

            return minMaxUserValues;
        }

        public static IEnumerable<double> GetXCoords(this ObservableCollection<ScatterDataPoint> numberPoints)
        {
            return numberPoints.Select(scatterDataPoint => scatterDataPoint.XValue);
        }

        public static IEnumerable<double> GetYCoords(this ObservableCollection<ScatterDataPoint> numberPoints)
        {
            return numberPoints.Select(scatterDataPoint => scatterDataPoint.YValue != null ? scatterDataPoint.YValue.Value : 0);
        }

        public static Dict<string, EvidenceTable> Merge(Dict<string, EvidenceTable> unmergedEvidenceSet, List<double> baseRowsList, Vertex selectedVertex, Network network)
        {
            var mergedEvidenceSet = new Dict<string, EvidenceTable>();
            var newList = new List<double>();

            if (baseRowsList != null)
            {
                newList = baseRowsList.ToList();
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

                    // If this is a point feature an no table contains it, then don't add.
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

                    if (mergedEvidenceRow.Equals(uniqueRow))
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

        public static Point GetPointOnChart(this RadCartesianChart chart, ScatterDataPoint scatterPoint)
        {
            return chart.ConvertDataToPoint(new DataTuple(scatterPoint.XValue, scatterPoint.YValue));
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

            return values.Any(table => table.Any(row => row.Equals(evidenceRow)));
        }

        private static Dict<string, double[]> GetEvidenceAverage(List<EvidenceRow> unmergedEvidenceRows)
        {
            var columnNames = unmergedEvidenceRows[0].GetDynamicMemberNames().ToList();
            var noOfstates = (unmergedEvidenceRows[0][columnNames[0]] as VertexEvidence).Value.Count();
            var combinedColumnValues = new Dict<string, double[]>();

            foreach (var columnName in columnNames)
            {
                combinedColumnValues.Add(columnName, new double[noOfstates]);

                foreach (var evidenceRow in unmergedEvidenceRows)
                {
                    var i = 0;
                    var evidence = evidenceRow[columnName] as VertexEvidence;
                    var value = evidence.Value;

                    while (i < noOfstates)
                    {
                        combinedColumnValues[columnName][i] += value[i];
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