using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Marv.Input
{
    public class LineDataTable : DataTable
    {
        public LineDataTable(string tableName) : base(tableName) {}

        public double GetMaximum()
        {
            return this.GetFromToValues().Max();
        }

        public double GetMinimum()
        {
            return this.GetFromToValues().Min();
        }

        public bool IsValid(DataRow row)
        {
            // Check if the given row is valid
            var from = row["From"];
            var to = row["To"];

            if (from != DBNull.Value && to != DBNull.Value)
            {
                return (double) from <= (double) to;
            }

            return true;
        }

        private IEnumerable<double> GetFromToValues()
        {
            var values = new List<double>();

            foreach (var row in this.Rows.Cast<DataRow>())
            {
                var from = row["From"];
                var to = row["To"];

                if (@from != DBNull.Value)
                {
                    values.Add((double) @from);
                }

                if (to != DBNull.Value)
                {
                    values.Add((double) to);
                }
            }
            return values;
        }
    }
}