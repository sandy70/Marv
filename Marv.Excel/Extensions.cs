using System;
using Microsoft.Office.Interop.Excel;

namespace Marv.ExcelNew
{
    public static class Extensions
    {
        public static Worksheet GetWorksheetOrNew(this Workbook workbook, string worksheetName)
        {
            Worksheet worksheet;

            try
            {
                worksheet = (Worksheet) workbook.Sheets[worksheetName];
            }
            catch (Exception exp)
            {
                worksheet = (Worksheet) workbook.Worksheets.Add();
                worksheet.Name = worksheetName;
            }

            return worksheet;
        }

        public static object Read(this Worksheet worksheet, int row, int col)
        {
            return ((Range) worksheet.Cells[row, col]).Value2;
        }

        public static string ReadText(this Worksheet worksheet, int row, int col)
        {
            var value = ((Range) worksheet.Cells[row, col]).Value2;

            if (value == null)
            {
                return null;
            }

            return value.ToString();
        }

        public static void Write(this Worksheet worksheet, int row, int col, object value, bool isBold = false)
        {
            ((Range) worksheet.Cells[row, col]).Value2 = value;
            ((Range) worksheet.Cells[row, col]).Font.Bold = isBold;
        }

        public static void WriteCol(this Worksheet worksheet, int col, object value, bool isBold = false)
        {
            ((Range) worksheet.Cells.Columns[col]).Value2 = value;
        }

        public static void WriteRow(this Worksheet worksheet, int row, object value, bool isBold = false)
        {
            ((Range) worksheet.Cells.Rows[row]).Value2 = value;
        }

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