using System.Collections.Generic;
using Marv.Common.Graph;
using Microsoft.Office.Interop.Excel;

namespace Marv_Excel
{
    public static class Extensions
    {
        public static void WriteHeader(this Worksheet worksheet, string fileName, IEnumerable<Vertex> selectedVertices, int nYears)
        {
            var row = 1;
            var col = 1;

            ((Range) worksheet.Cells[row, col]).Font.Bold = true;
            worksheet.Cells[row, col++] = "Network File";
            worksheet.Cells[row++, col] = fileName;

            col = 1;
            row++;

            worksheet.WriteValue(row, col, "Section Name", true);
            col++;

            worksheet.WriteValue(row, col, "Latitude", true);
            col++;

            worksheet.WriteValue(row, col, "Longitude", true);
            col += 2;

            if (selectedVertices != null)
            {
                foreach (var vertex in selectedVertices)
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

        public static void WriteVertexSkeleton(this Worksheet worksheet, int row, int col, Vertex vertex)
        {
            worksheet.WriteValue(row, col, "Value");
            row++;

            foreach (var state in vertex.States)
            {
                worksheet.WriteValue(row, col, state.Key, isText: true);
                row++;
            }
        }

        public static void WriteVertexSkeletons(this Worksheet worksheet, IEnumerable<Vertex> selectedVertices, int nYears)
        {
            var col = 5;

            foreach (var vertex in selectedVertices)
            {
                const int row = 5;
                worksheet.WriteVertexSkeleton(row, col, vertex);
                col += nYears;
                col += 2;
            }
        }
    }
}