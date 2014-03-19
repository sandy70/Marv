using Microsoft.Office.Interop.Excel;

namespace Marv_Excel
{
    public static class Extensions
    {
        public static void WriteValue(this Worksheet worksheet, int row, int col, object text, bool isBold = false, bool isText = false)
        {
            var range = (Range) worksheet.Cells[row, col];

            if (isText)
            {
                range.NumberFormat = "@";
            }

            range.Value2 = text;
            range.Font.Bold = isBold;
        }


    }
}