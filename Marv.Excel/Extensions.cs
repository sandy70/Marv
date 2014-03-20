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

        public static void Write(this Worksheet worksheet, SheetModel sheetModel)
        {
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

                for (var year = sheetModel.StartYear; year <= sheetModel.EndYear; year++)
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

                col += (sheetModel.EndYear - sheetModel.StartYear) + 3;
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

        public static SheetModel Read(this Worksheet worksheet)
        {
            var sheetModel = new SheetModel();

            var row = 1;
            var col = 1;

            var key = worksheet.ReadText(row, col);
            var value = worksheet.Read(row, col + 1);

            while (key != null)
            {
                sheetModel.SheetHeaders[key] = value;

                row++;
                key = worksheet.ReadText(row, col);
                value = worksheet.Read(row, col + 1);
            }

            // Go to next row
            row++;

            // This row should contain the column headers.
            var columnHeader = worksheet.ReadText(row, col);

            while (columnHeader != null)
            {
                sheetModel.ColumnHeaders.Add(columnHeader);

                col++;
                columnHeader = worksheet.ReadText(row, col);
            }

            var fileName = sheetModel.SheetHeaders["Network File"].ToString();
            var graph = Graph.Read(fileName);

            // For the vertex blocks we work with find.
            Range currentFind = null;
            Range firstFind = null;

            currentFind = worksheet.Cells.Find("Value");

            while (currentFind != null)
            {
                if (firstFind == null)
                {
                    firstFind = currentFind;
                }
                else if (currentFind.Address == firstFind.Address)
                {
                    break;
                }

                row = currentFind.Row;
                col = currentFind.Column;

                // Get vertexKey
                var vertexKey = worksheet.ReadText(sheetModel.SheetHeaders.Count + 2, col);
                var vertex = graph.Vertices[vertexKey];

                col++;
                var year = worksheet.Read(sheetModel.SheetHeaders.Count + 2, col);

                while (year != null)
                {
                    value = worksheet.Read(row, col);

                    if (value != null)
                    {
                        var evidence = EvidenceStringFactory.Create(value.ToString()).Parse(vertex);
                        sheetModel.ModelEvidence[Convert.ToInt32(year)][vertexKey] = evidence;
                    }
                    else
                    {
                        var evidenceArray = new double[vertex.States.Count];

                        for (int i = 0; i < evidenceArray.Length; i++)
                        {
                            value = worksheet.Read(row + i + 1, col);

                            if (value == null)
                            {
                                evidenceArray[i] = 0;
                            }
                            else
                            {
                                evidenceArray[i] = Convert.ToDouble(value);
                            }
                        }

                        var evidence = new SoftEvidence
                        {
                            Evidence = evidenceArray
                        };

                        sheetModel.ModelEvidence[Convert.ToInt32(year)][vertexKey] = evidence;
                    }

                    col++;
                    year = worksheet.Read(sheetModel.SheetHeaders.Count + 2, col);
                }

                currentFind = worksheet.Cells.FindNext(currentFind);
            }

            return sheetModel;
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