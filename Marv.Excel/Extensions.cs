using System;
using System.Collections.Generic;
using Marv.Common;
using Marv.Common.Graph;
using Microsoft.Office.Interop.Excel;

namespace Marv.Excel
{
    public static class Extensions
    {
        public static Worksheet GetWorksheetOrNew(this Workbook workbook, string worksheetName)
        {
            Worksheet worksheet;

            try
            {
                worksheet = workbook.Sheets[worksheetName];
            }
            catch (Exception)
            {
                worksheet = (Worksheet) Globals.ThisAddIn.Application.ActiveWorkbook.Worksheets.Add();
                worksheet.Name = worksheetName;
            }

            return worksheet;
        }

        public static void WriteVertexValues(this Worksheet worksheet, IEnumerable<Vertex> vertices, Dictionary<string, string, double> graphValue, string fileName, bool writeValues = true)
        {
            var rowIndex = 1;

            // Generate the static part
            worksheet.Cells[rowIndex, 1] = "File Name";
            worksheet.Cells[rowIndex++, 2] = fileName;

            worksheet.Cells[rowIndex++, 1] = "Name";
            worksheet.Cells[rowIndex++, 1] = "Units";
            worksheet.Cells[rowIndex++, 1] = "Description";
            worksheet.Cells[rowIndex++, 1] = "Value";

            var evidenceStartRowIndex = rowIndex;

            var colIndex = 4;

            foreach (var vertex in vertices)
            {
                // Set the header cells for this vertex
                worksheet.Cells[2, colIndex] = vertex.Key;
                worksheet.Cells[3, colIndex] = vertex.Units;
                worksheet.Cells[4, colIndex] = vertex.Description;

                rowIndex = evidenceStartRowIndex;

                var vertexValue = graphValue[vertex.Key];

                // Set the state cells for this vertex
                foreach (var state in vertex.States)
                {
                    // Prefixing with ' makes sure the value is formatted as text
                    worksheet.Cells[rowIndex, colIndex - 1].NumberFormat = "@";
                    worksheet.Cells[rowIndex, colIndex - 1] = state.Key;

                    if (writeValues)
                    {
                        worksheet.Cells[rowIndex, colIndex].NumberFormat = "@";
                        worksheet.Cells[rowIndex, colIndex] = vertexValue[state.Key];
                    }

                    rowIndex += 1;
                }

                colIndex += 2;
            }
        }
    }
}