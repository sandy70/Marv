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

        private void LineDataTable_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Add)
            {
                if (!DBNull.Value.Equals(e.Row["To"]))
                {
                    e.Row["From"] = e.Row["To"];
                }

                else if (!DBNull.Value.Equals(e.Row["From"]))
                {
                    e.Row["To"] = e.Row["From"];
                }
            }
        }
    }
}