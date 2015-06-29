using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Marv.Common;
using Marv.Common.Types;
using Telerik.Charting;
using Telerik.Windows.Controls;

namespace Marv.Input
{
    public static class Utils
    {
        public const double Infinity = 10E+09;

        public static double Distance(ScatterDataPoint p1, ScatterDataPoint p2)
        {
            if (p1.YValue != null && p2.YValue != null)
            {
                return Math.Sqrt(Math.Pow(p1.XValue - p2.XValue, 2) + Math.Pow((double) p1.YValue - (double) p2.YValue, 2));
            }
            return 0;
        }

        public static Dict<string, double> GetMinMaxUserValues(this EvidenceTable userTable)
        {
            var minUserValue = Infinity;
            double maxUserValue = 0;
            var minMaxUserValues = new Dict<string, double>();
            foreach (var row in userTable)
            {
                foreach (var dateTime in userTable.DateTimes)
                {
                    var evidence = row[dateTime.String()] as VertexEvidence;

                    if (evidence == null)
                    {
                        continue;
                    }
                    maxUserValue = Math.Max(maxUserValue, evidence.Params.Max());
                    minUserValue = Math.Min(minUserValue, evidence.Params.Min());

                    minMaxUserValues.Add("Maximum", maxUserValue);
                    minMaxUserValues.Add("Minimum", minUserValue);
                }
            }

            return minMaxUserValues;
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

        public static Dict<string, EvidenceTable> Merge(Dict<string, EvidenceTable> unmergedEvidenceSet)
        {
            var mergedEvidenceSet = new Dict<string, EvidenceTable>();
            var newList = new List<double>();

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

                    mergedEvidenceTable.AddUnique(evidenceRow);
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

        private static bool Contains(this Dict<string, EvidenceTable> evidenceSet, EvidenceRow evidenceRow)
        {
            var values = new ObservableCollection<EvidenceTable>();

            foreach (var kvp in evidenceSet)
            {
                values.Add(kvp.Value);
            }

            return values.Any(table => table.Any(row => row.Equals(evidenceRow)));
        }

        public static IEnumerable<double> GetXCoords(this ObservableCollection<ScatterDataPoint> numberPoints)
        {
            var coords = numberPoints.Select(scatterDataPoint => scatterDataPoint.XValue).ToList();
            IEnumerable<double> xCoords = coords;

            return xCoords;
        }

        public static IEnumerable<double> GetYCoords(this ObservableCollection<ScatterDataPoint> numberPoints)
        {
            var coords = numberPoints.Select(scatterDataPoint => scatterDataPoint.YValue).ToList();
           
            var newYCords = coords.Cast<double>().ToList();

            IEnumerable<double> yCoords = newYCords;
            return yCoords;
        }
    }
}