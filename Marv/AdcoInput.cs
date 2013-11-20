using Marv.Common;
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

        public static Dictionary<string, IEvidence> GetGraphEvidence(Graph graph, string fileName, string pipeName, string locationName)
        {
            using (var package = new ExcelPackage(new FileInfo(fileName)))
            {
                var graphEvidence = new Dictionary<string, IEvidence>();
                var nHeaderRows = 3;
                var sheet = package.Workbook.Worksheets.Single(worksheet => worksheet.Name == "data");

                var colIndex = 1;
                var colName = sheet.GetValue(1, colIndex);

                while (colName != null)
                {
                    var evidenceType = sheet.GetValue<string>(2, colIndex);

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

                            IEvidence evidence = null;
                            var evidenceString = sheet.GetValue<string>(rowIndex, colIndex);
                            var vertexKey = sheet.GetValue<string>(1, colIndex);

                            var vertex = graph.GetVertex(vertexKey);

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
                                evidence.EvidenceString = evidenceString;
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

                            IEvidence evidence = null;
                            var evidenceString = sheet.GetValue<string>(rowIndex, colIndex);
                            var vertexKey = sheet.GetValue<string>(1, colIndex);

                            var vertex = graph.GetVertex(vertexKey);

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

        public static ViewModelCollection<LocationCollection> Read(string fileName)
        {
            using (var package = new ExcelPackage(new FileInfo(fileName)))
            {
                var multiLocations = new ViewModelCollection<LocationCollection>();
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
                            Latitude = (double)sheet.GetValue(rowIndex, "Latitude"),
                            Longitude = (double)sheet.GetValue(rowIndex, "Longitude"),
                            Name = sheet.GetValue(rowIndex, "section inlet").ToString()
                        });
                    }

                    multiLocations.Add(multiLocation);

                    pipelineIndex++;
                }

                return multiLocations;
            }
        }

        private static IEvidence ParseDistribution(string evidenceString, Vertex vertex)
        {
            var parts = evidenceString.Trim()
                                      .Split(";".ToArray(), StringSplitOptions.RemoveEmptyEntries);

            double[] evidenceArray = new double[vertex.States.Count];

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
                            if (state.Range.Bounds(value))
                            {
                                evidenceArray[vertex.States.IndexOf(state)] += probability;
                            }
                        }
                    }
                    else
                    {
                        foreach (var state in vertex.States)
                        {
                            if (state.Key == partsOfPart[0])
                            {
                                evidenceArray[vertex.States.IndexOf(state)] += probability;
                            }
                        }
                    }
                }
                else
                {
                    throw new Smile.SmileException("");
                }
            }

            return new SoftEvidence
            {
                Evidence = evidenceArray
            };
        }

        private static IEvidence ParseRange(string evidenceString, Vertex vertex)
        {
            IEvidence evidence = null;

            var parts = evidenceString.Trim()
                                      .Split(":".ToArray(), StringSplitOptions.RemoveEmptyEntries);

            double minValue;
            double maxValue;

            if (Double.TryParse(parts[0], out minValue) && Double.TryParse(parts[1], out maxValue))
            {
                double[] evidenceArray = new double[vertex.States.Count];

                foreach (var state in vertex.States)
                {
                    if (maxValue < state.Range.Min)
                    {
                        // do nothing
                    }
                    else if (minValue > state.Range.Max)
                    {
                        // do nothing
                    }
                    else
                    {
                        if (minValue >= state.Range.Min && minValue <= state.Range.Max)
                        {
                            evidenceArray[vertex.States.IndexOf(state)] = (state.Range.Max - minValue) / (state.Range.Max - state.Range.Min);
                        }

                        if (maxValue >= state.Range.Min && maxValue <= state.Range.Max)
                        {
                            evidenceArray[vertex.States.IndexOf(state)] = (maxValue - state.Range.Min) / (state.Range.Max - state.Range.Min);
                        }

                        if (minValue <= state.Range.Min && maxValue >= state.Range.Max)
                        {
                            evidenceArray[vertex.States.IndexOf(state)] = 1;
                        }
                    }
                }

                evidence = new SoftEvidence
                {
                    Evidence = evidenceArray
                };
            }
            else
            {
                throw new Smile.SmileException("");
            }
            return evidence;
        }

        private static IEvidence ParseState(string evidenceString, Vertex vertex)
        {
            IEvidence evidence = null;

            int stateIndex = -1;

            foreach (var state in vertex.States)
            {
                if (state.Key == evidenceString)
                {
                    stateIndex = vertex.States.IndexOf(state);
                }
            }

            if (stateIndex >= 0)
            {
                evidence = new HardEvidence
                {
                    StateIndex = stateIndex
                };
            }
            else
            {
                double value;

                if (Double.TryParse(evidenceString, out value))
                {
                    double[] evidenceArray = new double[vertex.States.Count];

                    foreach (var state in vertex.States)
                    {
                        if (state.Range.Bounds(value))
                        {
                            evidenceArray[vertex.States.IndexOf(state)] = 1;
                        }
                    }

                    evidence = new SoftEvidence
                    {
                        Evidence = evidenceArray
                    };
                }
                else
                {
                    throw new Smile.SmileException("");
                }
            }
            return evidence;
        }
    }
}