using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Marv.Common;
using Marv.Common.Graph;
using Marv.Common.Map;
using OfficeOpenXml;
using Smile;

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

        public static void GetEvidence(string fileName, string pipeName, string locationName)
        {
            using (var package = new ExcelPackage(new FileInfo(fileName)))
            {
                var nHeaderRows = 3;
                var sheet = package.Workbook.Worksheets.Single(worksheet => worksheet.Name == "data");

                var colIndex = 1;
                var colName = sheet.GetValue(1, colIndex);

                while (colName != null)
                {
                    var evidenceType = sheet.GetValue(2, colIndex);

                    if (evidenceType != null)
                    {
                        if (evidenceType.ToString() == "Simple")
                        {
                            var rowIndex = nHeaderRows + 1;
                            var currentPipeName = sheet.GetValue(rowIndex, "R1C1");

                            while (currentPipeName.ToString() != pipeName)
                            {
                                currentPipeName = sheet.GetValue(++rowIndex, "R1C1");
                            }

                            var evidenceString = sheet.GetValue<string>(rowIndex, colIndex);
                        }
                    }

                    colName = sheet.GetValue(1, ++colIndex);
                }
            }
        }

        public static Dictionary<string, string, double> GetGraphEvidence(Graph graph, string fileName, string pipeName, string locationName)
        {
            using (var package = new ExcelPackage(new FileInfo(fileName)))
            {
                var graphEvidence = new Dictionary<string, string, double>();
                var nHeaderRows = 3;
                var sheet = package.Workbook.Worksheets.Single(worksheet => worksheet.Name == "data");

                var colIndex = 1;
                var colName = sheet.GetValue(1, colIndex);

                while (colName != null)
                {
                    var evidenceType = sheet.GetValue<string>(2, colIndex);

                    if (evidenceType != null)
                    {
                        if (evidenceType == "Simple")
                        {
                            var rowIndex = nHeaderRows + 1;
                            var currentPipeName = sheet.GetValue(rowIndex, "R1C1");

                            while (currentPipeName.ToString() != pipeName)
                            {
                                currentPipeName = sheet.GetValue(++rowIndex, "R1C1");
                            }

                            Dictionary<string, double> evidence = null;
                            var evidenceString = sheet.GetValue<string>(rowIndex, colIndex);
                            var vertexKey = sheet.GetValue<string>(1, colIndex);

                            var vertex = graph.Vertices[vertexKey];

                            if (evidenceString.Contains('?'))
                            {
                                // do nothing
                            }
                            else if (evidenceString.Contains(';'))
                            {
                                evidence = ParseDistribution(evidenceString, vertex);
                            }
                            else if (evidenceString.Contains(':'))
                            {
                                evidence = ParseRange(evidenceString, vertex);
                            }
                            else
                            {
                                evidence = ParseState(evidenceString, vertex);
                            }

                            if (evidence != null)
                            {
                                graphEvidence[vertexKey] = evidence;
                            }
                        }
                        else if (evidenceType == "Space")
                        {
                            var rowIndex = nHeaderRows + 1;
                            var currentPipeName = sheet.GetValue<string>(rowIndex, "R1C1");
                            var currentLocationName = sheet.GetValue<string>(rowIndex, "section inlet");

                            while (!(currentPipeName == pipeName && currentLocationName == locationName))
                            {
                                currentPipeName = sheet.GetValue<string>(++rowIndex, "R1C1");
                                currentLocationName = sheet.GetValue<string>(rowIndex, "section inlet");
                            }

                            Dictionary<string, double> evidence = null;
                            var evidenceString = sheet.GetValue<string>(rowIndex, colIndex);
                            var vertexKey = sheet.GetValue<string>(1, colIndex);

                            var vertex = graph.Vertices[vertexKey];

                            if (evidenceString == null)
                            {
                                evidenceString = "0";
                            }

                            if (evidenceString.Contains('?'))
                            {
                                // do nothing
                            }
                            else if (evidenceString.Contains(';'))
                            {
                                evidence = ParseDistribution(evidenceString, vertex);
                            }
                            else if (evidenceString.Contains(':'))
                            {
                                evidence = ParseRange(evidenceString, vertex);
                            }
                            else
                            {
                                evidence = ParseState(evidenceString, vertex);
                            }

                            if (evidence != null)
                            {
                                graphEvidence[vertexKey] = evidence;
                            }
                        }
                    }

                    colName = sheet.GetValue(1, ++colIndex);
                }

                return graphEvidence;
            }
        }

        public static object GetValue(this ExcelWorksheet sheet, int rowIndex, string columnName)
        {
            return sheet.GetValue(rowIndex, sheet.GetColumnIndex(columnName));
        }

        public static TResult GetValue<TResult>(this ExcelWorksheet sheet, int rowIndex, string columnName)
        {
            return sheet.GetValue<TResult>(rowIndex, sheet.GetColumnIndex(columnName));
        }

        public static ModelCollection<LocationCollection> Read(string fileName)
        {
            using (var package = new ExcelPackage(new FileInfo(fileName)))
            {
                var multiLocations = new ModelCollection<LocationCollection>();
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

                    var multiLocation = new LocationCollection();
                    multiLocation.Name = sheet.GetValue(pipelineStartRowIndex, "R1C1") as string;

                    var startYear = sheet.GetValue(pipelineStartRowIndex, "START");

                    multiLocation.Properties["StartYear"] = Convert.ToInt32(sheet.GetValue(pipelineStartRowIndex, "START"));

                    for (rowIndex = pipelineStartRowIndex; rowIndex < pipelineEndRowIndex; rowIndex++)
                    {
                        multiLocation.Add(new Location
                        {
                            Latitude = (double) sheet.GetValue(rowIndex, "Latitude"),
                            Longitude = (double) sheet.GetValue(rowIndex, "Longitude"),
                            Name = sheet.GetValue(rowIndex, "section inlet").ToString()
                        });
                    }

                    multiLocations.Add(multiLocation);

                    pipelineIndex++;
                }

                return multiLocations;
            }
        }

        private static Dictionary<string, double> ParseDistribution(string evidenceString, Vertex vertex)
        {
            var parts = evidenceString.Trim()
                .Split(";".ToArray(), StringSplitOptions.RemoveEmptyEntries);

            var vertexValue = new Dictionary<string, double>();

            foreach (var state in vertex.States)
            {
                vertexValue[state.Key] = 0;
            }

            foreach (var part in parts)
            {
                var partsOfPart = part.Trim()
                    .Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);

                double probability;

                if (Double.TryParse(partsOfPart[1], out probability))
                {
                    double value;

                    if (Double.TryParse(partsOfPart[0], out value))
                    {
                        foreach (var state in vertex.States)
                        {
                            if (state.Contains(value))
                            {
                                vertexValue[state.Key] += probability;
                            }
                        }
                    }
                    else
                    {
                        foreach (var state in vertex.States)
                        {
                            if (state.Key == partsOfPart[0])
                            {
                                vertexValue[state.Key] += probability;
                            }
                        }
                    }
                }
                else
                {
                    throw new SmileException("");
                }
            }

            return vertexValue;
        }

        private static Dictionary<string, double> ParseRange(string evidenceString, Vertex vertex)
        {
            var evidence = new Dictionary<string, double>();

            var parts = evidenceString.Trim()
                .Split(":".ToArray(), StringSplitOptions.RemoveEmptyEntries);

            double minValue;
            double maxValue;

            if (Double.TryParse(parts[0], out minValue) && Double.TryParse(parts[1], out maxValue))
            {
                foreach (var state in vertex.States)
                {
                    if (maxValue < state.Min)
                    {
                        // do nothing
                    }
                    else if (minValue > state.Max)
                    {
                        // do nothing
                    }
                    else
                    {
                        if (minValue >= state.Min && minValue <= state.Max)
                        {
                            evidence[state.Key] = (state.Max - minValue)/(state.Max - state.Min);
                        }

                        if (maxValue >= state.Min && maxValue <= state.Max)
                        {
                            evidence[state.Key] = (maxValue - state.Min)/(state.Max - state.Min);
                        }

                        if (minValue <= state.Min && maxValue >= state.Max)
                        {
                            evidence[state.Key] = 1;
                        }
                    }
                }
            }
            else
            {
                throw new SmileException("");
            }

            return evidence;
        }

        private static Dictionary<string, double> ParseState(string evidenceString, Vertex vertex)
        {
            var evidence = new Dictionary<string, double>();

            if (vertex.States.Count(state => state.Key == evidenceString) == 1)
            {
                evidence[evidenceString] = 1;
            }
            else
            {
                double value;

                if (Double.TryParse(evidenceString, out value))
                {
                    foreach (var state in vertex.States.Where(state => state.Contains(value)))
                    {
                        evidence[state.Key] = 1;
                    }
                }
                else
                {
                    throw new SmileException("");
                }
            }

            return evidence;
        }
    }
}