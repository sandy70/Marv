using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Marv.Common;
using Marv.Common.Graph;
using Marv.Common.Map;
using OfficeOpenXml;

namespace Marv
{
    public class PipelineInput
    {
        private readonly Dictionary<string, int> columnIndices = new Dictionary<string, int>();
        private readonly ExcelWorksheet sheet;

        public PipelineInput(string fileName)
        {
            using (var package = new ExcelPackage(new FileInfo(fileName)))
            {
                this.sheet = package.Workbook.Worksheets.Single(worksheet => worksheet.Name == "data");
            }
        }

        public int GetColumnIndex(string columnName)
        {
            if (this.columnIndices.ContainsKey(columnName))
            {
                return this.columnIndices[columnName];
            }

            // Column indices are 1 based.
            var columnIndex = 1;
            var value = this.sheet.GetValue(1, columnIndex);

            while (value != null)
            {
                if (value.Equals(columnName))
                {
                    this.columnIndices[columnName] = columnIndex;
                    return columnIndex;
                }

                value = this.sheet.GetValue(1, ++columnIndex);
            }

            return -1;
        }

        public Dictionary<string, string, double> GetGraphEvidence(Graph graph, string pipeName, string locationName)
        {
            var graphEvidence = new Dictionary<string, string, double>();
            var nHeaderRows = 3;

            var colIndex = 1;
            var colName = this.sheet.GetValue(1, colIndex);

            while (colName != null)
            {
                var evidenceType = this.sheet.GetValue<string>(2, colIndex);

                if (evidenceType != null)
                {
                    if (evidenceType == "Simple")
                    {
                        var rowIndex = nHeaderRows + 1;
                        var currentPipeName = this.GetValue(rowIndex, "R1C1");

                        while (currentPipeName.ToString() != pipeName)
                        {
                            currentPipeName = this.GetValue(++rowIndex, "R1C1");
                        }

                        var evidenceString = this.sheet.GetValue<string>(rowIndex, colIndex);

                        if (evidenceString == null)
                        {
                            evidenceString = "0";
                        }

                        var vertexKey = this.sheet.GetValue<string>(1, colIndex);
                        var vertex = graph.Vertices[vertexKey];

                        var evidence = EvidenceStringFactory.Create(evidenceString).Parse(vertex.States, evidenceString);

                        if (evidence != null)
                        {
                            graphEvidence[vertexKey] = evidence;
                        }
                    }
                    else if (evidenceType == "Space")
                    {
                        var rowIndex = nHeaderRows + 1;
                        var currentPipeName = this.GetValue<string>(rowIndex, "R1C1");
                        var currentLocationName = this.GetValue<string>(rowIndex, "section inlet");

                        while (!(currentPipeName == pipeName && currentLocationName == locationName))
                        {
                            currentPipeName = this.GetValue<string>(++rowIndex, "R1C1");
                            currentLocationName = this.GetValue<string>(rowIndex, "section inlet");
                        }

                        var evidenceString = this.sheet.GetValue<string>(rowIndex, colIndex);

                        if (evidenceString == null)
                        {
                            evidenceString = "0";
                        }

                        var vertexKey = this.sheet.GetValue<string>(1, colIndex);
                        var vertex = graph.Vertices[vertexKey];

                        var evidence = EvidenceStringFactory.Create(evidenceString).Parse(vertex.States, evidenceString);

                        if (evidence != null)
                        {
                            graphEvidence[vertexKey] = evidence;
                        }
                    }
                }

                colName = this.sheet.GetValue(1, ++colIndex);
            }

            return graphEvidence;
        }

        public object GetValue(int rowIndex, string columnName)
        {
            return this.sheet.GetValue(rowIndex, this.GetColumnIndex(columnName));
        }

        public TResult GetValue<TResult>(int rowIndex, string columnName)
        {
            return this.sheet.GetValue<TResult>(rowIndex, this.GetColumnIndex(columnName));
        }

        public ModelCollection<LocationCollection> ReadPipelines()
        {
            var multiLocations = new ModelCollection<LocationCollection>();
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
                multiLocation.Key = multiLocation.Name;

                multiLocation.Properties["StartYear"] = Convert.ToInt32(this.GetValue(pipelineStartRowIndex, "START"));

                for (rowIndex = pipelineStartRowIndex; rowIndex < pipelineEndRowIndex; rowIndex++)
                {
                    multiLocation.Add(new Location
                    {
                        Key = this.GetValue(rowIndex, "section inlet").ToString(),
                        Latitude = (double) this.GetValue(rowIndex, "Latitude"),
                        Longitude = (double) this.GetValue(rowIndex, "Longitude"),
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