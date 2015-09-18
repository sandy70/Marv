using Marv.Common.Types;
using Telerik.Windows.Documents.Spreadsheet.Model;

namespace Marv.Input
{
    public class WorkSheetTemplate
    {
        private readonly Dict<string, int> columns = new Dict<string, int>();

        public Worksheet WorkSheet { get; set; }

        public WorkSheetTemplate(Worksheet worksheet)
        {
            this.WorkSheet = worksheet;
        }

        public void AddColumn(string colName, int row, int colWidth)
        {
            var allotedCells = 0;
            foreach (var kvp in columns)
            {
                allotedCells += kvp.Value;
            }
            var startCell = new CellIndex(row, allotedCells);
            var endCell = new CellIndex(row, allotedCells + colWidth - 1);

            this.WorkSheet.Cells[startCell, endCell].Merge();
            this.WorkSheet.Cells[startCell, endCell].SetIsBold(true);

            this.WorkSheet.Cells[startCell, endCell].SetValue(colName);

            columns.Add(colName, colWidth);
        }

        public void AddSubColumn(string parentColumnName, string subColumnName, int rowIndex, int subColWidth)
        {
            var allotedCellsInCol = 0;
            var colIndex = 0;
            foreach (var kvp in columns)
            {
                if (kvp.Key == parentColumnName)
                {
                    break;
                }
                colIndex += kvp.Value;
            }

            while (WorkSheet.Cells[rowIndex, colIndex + allotedCellsInCol].GetValue().Value.ValueType != CellValueType.Empty)
            {
                allotedCellsInCol ++;
            }

            var startCell = new CellIndex(rowIndex, colIndex + allotedCellsInCol);
            var endCell = new CellIndex(rowIndex, startCell.ColumnIndex + subColWidth - 1);

            WorkSheet.Cells[startCell, endCell].Merge();
            this.WorkSheet.Cells[startCell, endCell].SetValue(subColumnName);
        }

        public Dict<string, int> GetColumns()
        {
            return this.columns;
        }

        public int GetSubColumnPosition(string parentColName,  int rowPosition)
        {
            var subColumnPosition = 0;
            var colIndex = 0;
            foreach (var kvp in columns)
            {
                if (kvp.Key == parentColName)
                {
                    break;
                }
                colIndex += kvp.Value;
            }

            while (WorkSheet.Cells[rowPosition, colIndex + subColumnPosition].GetValue().Value.ValueType != CellValueType.Empty)
            {
                subColumnPosition++;
            }

            return colIndex + subColumnPosition;
        }
    }
}