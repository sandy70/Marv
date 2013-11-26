using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Marv.Common
{
    public class PipelineInput
    {
        private Dictionary<string, int> columnIndices = new Dictionary<string, int>();
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
            if (this.columnIndices.ContainsKey(columnName))
            {
                return this.columnIndices[columnName];
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
                        this.columnIndices[columnName] = columnIndex;
                        return columnIndex;
                    }

                    value = this.sheet.GetValue(1, ++columnIndex);
                }

                return -1;
            }
        }

        public Dictionary<string, IEvidence> GetGraphEvidence(Graph graph, string pipeName, string locationName)
        {
            var graphEvidence = new Dictionary<string, IEvidence>();
            var nHeaderRows = 3;

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
                        var currentPipeName = this.GetValue(rowIndex, "R1C1");

                        while (currentPipeName.ToString() != pipeName)
                        {
                            currentPipeName = this.GetValue(++rowIndex, "R1C1");
                        }

                        var evidenceString = sheet.GetValue<string>(rowIndex, colIndex);

                        if (evidenceString == null)
                        {
                            evidenceString = "0";
                        }

                        var vertexKey = sheet.GetValue<string>(1, colIndex);
                        var vertex = graph.Vertices[vertexKey];

                        var evidence = EvidenceStringFactory.Create(evidenceString).Parse(vertex);

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

                        var evidenceString = sheet.GetValue<string>(rowIndex, colIndex);

                        if (evidenceString == null)
                        {
                            evidenceString = "0";
                        }

                        var vertexKey = sheet.GetValue<string>(1, colIndex);
                        var vertex = graph.Vertices[vertexKey];

                        var evidence = EvidenceStringFactory.Create(evidenceString).Parse(vertex);

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

        public object GetValue(int rowIndex, string columnName)
        {
            return this.sheet.GetValue(rowIndex, this.GetColumnIndex(columnName));
        }

        public TResult GetValue<TResult>(int rowIndex, string columnName)
        {
            return this.sheet.GetValue<TResult>(rowIndex, this.GetColumnIndex(columnName));
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
                multiLocation.Key = multiLocation.Name;

                var startYear = this.GetValue(pipelineStartRowIndex, "START");

                multiLocation.Properties["StartYear"] = Convert.ToInt32(this.GetValue(pipelineStartRowIndex, "START"));

                for (rowIndex = pipelineStartRowIndex; rowIndex < pipelineEndRowIndex; rowIndex++)
                {
                    multiLocation.Add(new Location
                    {
                        Key = this.GetValue(rowIndex, "section inlet").ToString(),
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

        private IEvidence ParseDistribution(string evidenceString, Vertex vertex)
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

        private IEvidence ParseRange(string evidenceString, Vertex vertex)
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

        private IEvidence ParseState(string evidenceString, Vertex vertex)
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