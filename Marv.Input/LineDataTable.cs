using System;
using System.Data;

namespace Marv.Input
{
    public class LineDataTable : DataTable
    {
        public LineDataTable(string tableName) : base(tableName)
        {
            this.RowChanged += LineDataTable_RowChanged;
        }

        public double GetMaximum()
        {
            var max = double.MinValue;

            foreach (DataRow row in this.Rows)
            {
                var value = Math.Max((double) row["From"], (double) row["To"]);

                if (value > max)
                {
                    max = value;
                }
            }

            return max;
        }

        public double GetMinimum()
        {
            var min = double.MaxValue;

            foreach (DataRow row in this.Rows)
            {
                var value = Math.Min((double) row["From"], (double) row["To"]);

                if (value < min)
                {
                    min = value;
                }
            }

            return min;
        }

        private void LineDataTable_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Add)
            {
                Console.WriteLine("row added");
                /*
                if (!DBNull.Value.Equals(e.Row["To"]))
                {
                    e.Row["From"] = e.Row["To"];
                }

                else if (!DBNull.Value.Equals(e.Row["From"]))
                {
                    e.Row["To"] = e.Row["From"];
                }*/
            }
        }
    }
}