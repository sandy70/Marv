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

namespace Marv.Input
{
    public static class Utils
    {
        public const string MaxInterpolatorLine = "MaximumLine";
        public const string MinInterpolatorLine = "MinimumLine";
        public const double MinusInfinity = 10E-09;
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

        public static Point GetPointOnChart(this RadCartesianChart chart, ScatterDataPoint scatterPoint)
        {
            return chart.ConvertDataToPoint(new DataTuple(scatterPoint.XValue, scatterPoint.YValue));
        }

        public static ScatterDataPoint GetScatterDataPoint(this RadCartesianChart chart, Point position)
        {
            var data = chart.ConvertPointToData(position);

            var selectedDataPoint = new ScatterDataPoint
            {
                XValue = (double) data.FirstValue,
                YValue = (double) data.SecondValue
            };

            return selectedDataPoint;
        }

        public static IEnumerable<double> GetXCoords(this ObservableCollection<ScatterDataPoint> numberPoints)
        {
            return numberPoints.Select(scatterDataPoint => scatterDataPoint.XValue);
        }

        public static IEnumerable<double> GetYCoords(this ObservableCollection<ScatterDataPoint> numberPoints)
        {
            return numberPoints.Select(scatterDataPoint => scatterDataPoint.YValue != null ? scatterDataPoint.YValue.Value : 0);
        }

        public static Dict<string, EvidenceTable> Merge(Dict<string, EvidenceTable> unmergedEvidenceSet, List<double> baseRowsList)
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

                    // multiple inputs for a single point feature is allowed 
                    if (unmergedEvidenceSet.Contains(evidenceRow) && newList[i] == newList[i + 1])
                    {
                        mergedEvidenceTable.Add(evidenceRow);
                    }
                    else
                    {
                        mergedEvidenceTable.AddUnique(evidenceRow);
                    }
                }

                foreach (var mergedEvidenceRow in mergedEvidenceTable)
                {
                    var unmergedEvidenceRow = unmergedEvidenceTable.FirstOrDefault(row => row.Contains(mergedEvidenceRow));
                    var columnNames = mergedEvidenceRow.GetDynamicMemberNames().ToList();

                    foreach (var columnName in columnNames)
                    {
                        mergedEvidenceRow[columnName] = unmergedEvidenceRow == null ? new VertexEvidence { Type = VertexEvidenceType.Null } : unmergedEvidenceRow[columnName];
                    }
                }

                mergedEvidenceSet.Add(unmergeEvidenceTableKey, mergedEvidenceTable);
            }

            return mergedEvidenceSet;
        }

        public static Dict<string, EvidenceTable> UpdateInterpolatedData(this Dict<string, EvidenceTable> mergedEvidenceSet, Dict<string, EvidenceTable> interpolatedDataSet)
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

        private static bool Contains(this Dict<string, EvidenceTable> evidenceSet, EvidenceRow evidenceRow)
        {
            var values = new ObservableCollection<EvidenceTable>();

            foreach (var kvp in evidenceSet)
            {
                values.Add(kvp.Value);
            }

            return values.Any(table => table.Any(row => row.Equals(evidenceRow)));
        }

        public static bool IsWithInRange(this InterpolatorDataPoints currentInterpolatorDataPoints)
        {
            var currentLine = currentInterpolatorDataPoints;

            var currentMax = currentLine.GetNumberPoints(Utils.MaxInterpolatorLine);
            var currentMode = currentLine.GetNumberPoints(Utils.ModeInterpolatorLine);
            var currentMin = currentLine.GetNumberPoints(Utils.MinInterpolatorLine);

            var maxLinInterpolator = new LinearInterpolator(currentMax.GetXCoords(), currentMax.GetYCoords());
            var modeLinInterpolator = new LinearInterpolator(currentMode.GetXCoords(), currentMode.GetYCoords());
            var minLinInterpolator = new LinearInterpolator(currentMin.GetXCoords(), currentMin.GetYCoords());

            if (currentMax.Any(scatterPoint => !(scatterPoint.YValue > modeLinInterpolator.Eval(scatterPoint.XValue))))
            {
                return false;
            }

            if (currentMode.Any(scatterPoint => !(maxLinInterpolator.Eval(scatterPoint.XValue) > scatterPoint.YValue &&
                                                  scatterPoint.YValue > minLinInterpolator.Eval(scatterPoint.XValue))))
            {
                return false;
            }

            if (currentMin.Any(scatterPoint => !(modeLinInterpolator.Eval(scatterPoint.XValue) > scatterPoint.YValue)))
            {
                return false;
            }

            return true;
        }
    }
}