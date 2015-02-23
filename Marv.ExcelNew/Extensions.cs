using Microsoft.Office.Interop.Excel;

namespace Marv.ExcelNew
{
    public static class Extensions
    {
        public static void Write(this Worksheet worksheet, int row, int col, object value)
        {
            ((Range) worksheet.Cells[row, col]).Value2 = value;
        }
    }
}