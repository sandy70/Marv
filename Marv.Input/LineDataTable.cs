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
                var index = this.Rows.IndexOf(e.Row);

                if (index == 0)
                {
                    e.Row["From"] = 0;
                    e.Row["To"] = 0;
                }
                else
                {
                    var previousRow = this.Rows[index - 1];
                    e.Row["From"] = previousRow["To"];
                    e.Row["To"] = previousRow["To"];
                }
            }
        }
    }
}