using Marv.Common;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Marv
{
    public class PipelineInput
    {
        private Dict<string, int> columnIndexForColumnName = new Dict<string, int>();
        private ExcelWorksheet sheet = null;

        public PipelineInput(string fileName)
        {
            using (var package = new ExcelPackage(new FileInfo(fileName)))
            {
                this.sheet = package.Workbook.Worksheets.Single(worksheet => worksheet.Name == "data");
            }
        }

        public int GetColumnIndex(string columnName)
        {
            if (this.columnIndexForColumnName.ContainsKey(columnName))
            {
                return this.columnIndexForColumnName[columnName];
            }
            else
            {
                // Column indices are 1 based.
                var columnIndex = 1;
                var value = this.sheet.GetValue(1, columnIndex);

                while (value != null)
                {
                    if (value.Equals(columnName))
                    {
                        this.columnIndexForColumnName[columnName] = columnIndex;
                        return columnIndex;
                    }

                    value = this.sheet.GetValue(1, ++columnIndex);
                }

                return -1;
            }
        }

        public object GetValue(int rowIndex, string columnName)
        {
            return this.sheet.GetValue(rowIndex, sheet.GetColumnIndex(columnName));
        }

        public TResult GetValue<TResult>(int rowIndex, string columnName)
        {
            return this.sheet.GetValue<TResult>(rowIndex, sheet.GetColumnIndex(columnName));
        }

        public ViewModelCollection<LocationCollection> ReadPipelines()
        {
            var multiLocations = new ViewModelCollection<LocationCollection>();
            var nHeaderRows = 3;
            var pipelineStartRowIndices = new List<int>();
            var rowIndex = nHeaderRows + 1;
            var pipelineNameColumn = "R1C1";

            var name = this.GetValue(rowIndex, pipelineNameColumn);

            while (name != null)
            {
                if (rowIndex == nHeaderRows + 1 || name != this.GetValue(rowIndex - 1, pipelineNameColumn))
                {
                    pipelineStartRowIndices.Add(rowIndex);
                }

                name = this.GetValue(++rowIndex, pipelineNameColumn);
            }

            pipelineStartRowIndices.Add(rowIndex);

            var pipelineIndex = 1;

            while (pipelineIndex < pipelineStartRowIndices.Count)
            {
                var pipelineStartRowIndex = pipelineStartRowIndices[pipelineIndex - 1];
                var pipelineEndRowIndex = pipelineStartRowIndices[pipelineIndex];

                var multiLocation = new LocationCollection();
                multiLocation.Name = this.GetValue(pipelineStartRowIndex, pipelineNameColumn) as string;

                var startYear = this.GetValue(pipelineStartRowIndex, "START");

                multiLocation["StartYear"] = Convert.ToInt32(this.GetValue(pipelineStartRowIndex, "START"));

                for (rowIndex = pipelineStartRowIndex; rowIndex < pipelineEndRowIndex; rowIndex++)
                {
                    multiLocation.Add(new Location
                    {
                        Latitude = (double)this.GetValue(rowIndex, "Latitude"),
                        Longitude = (double)this.GetValue(rowIndex, "Longitude"),
                        Name = this.GetValue(rowIndex, "section inlet").ToString()
                    });
                }

                multiLocations.Add(multiLocation);

                pipelineIndex++;
            }

            return multiLocations;
        }
    }
}