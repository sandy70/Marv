using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Marv.Common;
using Marv.Common.Types;

namespace Marv.Input
{
    public static class Utils
    {
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
    }
}