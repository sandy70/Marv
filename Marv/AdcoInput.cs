using LibPipeline;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Marv
{
    public static class AdcoInput
    {
        public static int GetColumnIndex(this ExcelWorksheet sheet, string columnName)
        {
            // Column indices are 1 based.
            var columnIndex = 1;
            var value = sheet.GetValue(1, columnIndex);

            while (value != null)
            {
                if (value.Equals(columnName))
                {
                    return columnIndex;
                }

                value = sheet.GetValue(1, ++columnIndex);
            }

            return -1;
        }

        public static object GetValue(this ExcelWorksheet sheet, int rowIndex, string columnName)
        {
            return sheet.GetValue(rowIndex, sheet.GetColumnIndex(columnName));
        }

        public static SelectableCollection<MultiLocation> Read()
        {
            using (var package = new ExcelPackage(new FileInfo(@"D:\Data\ADCO02\ADCO 6.xlsx")))
            {
                var multiLocations = new SelectableCollection<MultiLocation>();
                var nHeaderRows = 3;
                var pipelineStartRowIndices = new List<int>();
                var rowIndex = nHeaderRows + 1;
                var sheet = package.Workbook.Worksheets.Single(worksheet => worksheet.Name == "data");

                var nameColumnIndex = sheet.GetColumnIndex("R1C1");
                var name = sheet.GetValue(rowIndex, nameColumnIndex);

                while (name != null)
                {
                    if (rowIndex == nHeaderRows + 1 || name != sheet.GetValue(rowIndex - 1, nameColumnIndex))
                    {
                        pipelineStartRowIndices.Add(rowIndex);
                    }

                    name = sheet.GetValue(++rowIndex, nameColumnIndex);
                }

                pipelineStartRowIndices.Add(rowIndex);

                var pipelineIndex = 1;

                while (pipelineIndex < pipelineStartRowIndices.Count)
                {
                    var pipelineStartRowIndex = pipelineStartRowIndices[pipelineIndex - 1];
                    var pipelineEndRowIndex = pipelineStartRowIndices[pipelineIndex];

                    var multiLocation = new MultiLocation();
                    multiLocation.Name = sheet.GetValue(pipelineStartRowIndex, "R1C1") as string;
                    multiLocation["StartYear"] = sheet.GetValue(pipelineStartRowIndex, "START");

                    for (rowIndex = pipelineStartRowIndex; rowIndex < pipelineEndRowIndex; rowIndex++)
                    {
                        multiLocation.Add(new Location
                        {
                            Latitude = (double)sheet.GetValue(rowIndex, "Latitude"),
                            Longitude = (double)sheet.GetValue(rowIndex, "Longitude")
                        });
                    }

                    multiLocations.Add(multiLocation);

                    pipelineIndex++;
                }

                return multiLocations;
            }
        }
    }
}