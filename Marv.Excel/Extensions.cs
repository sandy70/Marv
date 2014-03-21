using System;
using System.Collections.Generic;
using System.Linq;
using Marv.Common.Graph;
using Microsoft.Office.Interop.Excel;

namespace Marv_Excel
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
            catch (Exception)
            {
                worksheet = (Worksheet) workbook.Worksheets.Add();
                worksheet.Name = worksheetName;
            }

            return worksheet;
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
    }
}