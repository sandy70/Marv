using System;
using System.Collections.Generic;
using System.Linq;
using Marv.Common.Graph;
using Microsoft.Office.Interop.Excel;

namespace Marv_Excel
{
    public static class Extensions
    {
        public const int HeaderRows = 5;
        public const int HeaderCols = 4;

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

        public static void WriteSkeleton(this Worksheet worksheet, string fileName, IEnumerable<Vertex> selectedVertices, int nYears)
        {
            var sheetModel = new SheetModel();
            sheetModel.SheetHeaders["Network File"] = fileName;
            sheetModel.SheetHeaders["Years"] = nYears;

            sheetModel.ColumnHeaders.Add("Sections Name");
            sheetModel.ColumnHeaders.Add("Latitude");
            sheetModel.ColumnHeaders.Add("Longitude");

            sheetModel.Vertices = selectedVertices.ToList();

            var row = 1;
            var col = 1;

            foreach (var key in sheetModel.SheetHeaders.Keys)
            {
                worksheet.WriteValue(row, col, key, true, true);
                worksheet.WriteValue(row, col + 1, sheetModel.SheetHeaders[key], isText: true);
                row++;
            }

            // Leave blank row
            row++;

            foreach (var columnHeader in sheetModel.ColumnHeaders)
            {
                worksheet.WriteValue(row, col, columnHeader, true, true);
                col++;
            }

            // Leave blank column
            col++;

            foreach (var vertex in sheetModel.Vertices)
            {
                worksheet.WriteValue(row, col, vertex.Key, true);
                col++;

                for (var year = 0; year < nYears; year++)
                {
                    worksheet.WriteValue(row, col, year, isText: true);
                    col++;
                }

                col++;
            }

            // ColumnHeaders.Count + Blank Line + 1
            col = sheetModel.ColumnHeaders.Count + 2;

            foreach (var vertex in sheetModel.Vertices)
            {
                // SheetHeaders.Count + Blank Line + ColumnHeader Lines + Blank Line + 1
                row = sheetModel.SheetHeaders.Count + 4;

                worksheet.WriteValue(row, col, "Value");
                row++;

                foreach (var state in vertex.States)
                {
                    worksheet.WriteValue(row, col, state.Key, isText: true);
                    row++;
                }

                col += nYears + 2;
            }
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