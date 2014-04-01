using System;
using System.Collections.Generic;
using System.Linq;
using Marv.Common;
using Marv.Common.Graph;
using Microsoft.Office.Interop.Excel;

namespace Marv_Excel
{
    public class SheetModel
    {
        private List<string> columnHeaders = new List<string>();
        private int endYear;
        private Graph graph;
        private Dictionary<string, int, string, string, double> lineEvidence = new Dictionary<string, int, string, string, double>();
        private Dictionary<string, int, string, string, double> lineValue = new Dictionary<string,int,string,string,double>();
        private Dictionary<int, string, string, double> modelEvidence = new Dictionary<int, string, string, double>();
        private Dictionary<int, string, string, double> modelValue;
        private Dictionary<string, object> sheetHeaders = new Dictionary<string, object>();
        private int startYear;
        private IEnumerable<Vertex> vertices;

        public List<string> ColumnHeaders
        {
            get
            {
                return columnHeaders;
            }
            set
            {
                columnHeaders = value;
            }
        }

        public int EndYear
        {
            get
            {
                return endYear;
            }
            set
            {
                endYear = value;
            }
        }

        public Graph Graph
        {
            get
            {
                return graph;
            }
            set
            {
                graph = value;
            }
        }

        public Dictionary<string, int, string, string, double> LineEvidence
        {
            get
            {
                return this.lineEvidence;
            }

            set
            {
                this.lineEvidence = value;
            }
        }

        public Dictionary<string, int, string, string, double> LineValue
        {
            get
            {
                return lineValue;
            }

            set
            {
                lineValue = value;
            }
        }

        public Dictionary<int, string, string, double> ModelEvidence
        {
            get
            {
                return modelEvidence;
            }

            set
            {
                modelEvidence = value;
            }
        }

        public Dictionary<int, string, string, double> ModelValue
        {
            get
            {
                return modelValue;
            }
            set
            {
                modelValue = value;
            }
        }

        public Dictionary<string, object> SheetHeaders
        {
            get
            {
                return sheetHeaders;
            }

            set
            {
                sheetHeaders = value;
            }
        }

        public int StartYear
        {
            get
            {
                return startYear;
            }
            set
            {
                startYear = value;
            }
        }

        public IEnumerable<Vertex> Vertices
        {
            get
            {
                if (this.vertices == null)
                {
                    return this.Graph.Vertices;
                }

                return vertices;
            }

            set
            {
                vertices = value;
            }
        }

        public static SheetModel Read(Worksheet worksheet)
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

            sheetModel.StartYear = Convert.ToInt32(sheetModel.SheetHeaders["Start Year"]);
            sheetModel.EndYear = Convert.ToInt32(sheetModel.SheetHeaders["End Year"]);

            var fileName = sheetModel.SheetHeaders["Network File"].ToString();
            sheetModel.Graph = Graph.Read(fileName);

            // For the vertex blocks we work with find.
            Range firstFind = null;
            var currentFind = worksheet.Cells.Find("Value");

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
                var vertex = sheetModel.Graph.Vertices[vertexKey];

                col++;
                var year = worksheet.Read(sheetModel.SheetHeaders.Count + 2, col);

                var sectionId = worksheet.ReadText(row, 1);

                while (year != null)
                {
                    value = worksheet.Read(row, col);

                    if (value != null)
                    {
                        var evidence = EvidenceStringFactory.Create(value.ToString()).Parse(vertex);
                        sheetModel.ModelEvidence[Convert.ToInt32(year), vertexKey] = evidence;
                        sheetModel.LineEvidence[sectionId, Convert.ToInt32(year), vertexKey] = evidence;
                    }
                    else
                    {
                        var evidence = new Dictionary<string, double>();

                        foreach (var state in vertex.States)
                        {
                            var i = vertex.States.IndexOf(state);

                            value = worksheet.Read(row + i + 1, col);

                            if (value == null)
                            {
                                evidence[state.Key] = 0;
                            }
                            else
                            {
                                evidence[state.Key] = Convert.ToDouble(value);
                            }
                        }

                        sheetModel.ModelEvidence[Convert.ToInt32(year), vertexKey] = evidence;
                        sheetModel.LineEvidence[sectionId, Convert.ToInt32(year), vertexKey] = evidence;
                    }

                    col++;
                    year = worksheet.Read(sheetModel.SheetHeaders.Count + 2, col);
                }

                currentFind = worksheet.Cells.FindNext(currentFind);
            }

            return sheetModel;
        }

        public void Run()
        {
            foreach (var sectionId in this.LineEvidence.Keys)
            {
                this.LineValue[sectionId] = this.Graph.Run(this.LineEvidence[sectionId], this.startYear, this.EndYear);
            }
        }

        public void Write(Worksheet worksheet)
        {
            var row = 1;
            var col = 1;

            foreach (var key in this.SheetHeaders.Keys)
            {
                worksheet.WriteValue(row, col, key, true, true);
                worksheet.WriteValue(row, col + 1, this.SheetHeaders[key], isText: true);
                row++;
            }

            // Leave blank row
            row++;

            foreach (var columnHeader in this.ColumnHeaders)
            {
                worksheet.WriteValue(row, col, columnHeader, true, true);
                col++;
            }

            // Leave blank column
            col++;

            foreach (var vertex in this.Vertices)
            {
                worksheet.WriteValue(row, col, vertex.Key, true);
                col++;

                for (var year = this.StartYear; year <= this.EndYear; year++)
                {
                    worksheet.WriteValue(row, col, year, isText: true);
                    col++;
                }

                col++;
            }

            // SheetHeaders.Count + Blank Line + ColumnHeader Lines + Blank Line + 1
            var sectionRow = this.SheetHeaders.Count + 4;

            foreach (var sectionId in this.LineValue.Keys)
            {
                // ColumnHeaders.Count + Blank Line + 1
                col = this.ColumnHeaders.Count + 2;

                worksheet.WriteValue(sectionRow, 1, sectionId, isText: true);

                var modelValue = this.LineValue[sectionId];

                foreach (var vertex in this.Vertices)
                {
                    row = sectionRow;

                    worksheet.WriteValue(row, col, "Value");
                    row++;

                    foreach (var state in vertex.States)
                    {
                        worksheet.WriteValue(row, col, state.Key, isText: true);
                        row++;
                    }

                    col++;

                    for (var year = this.StartYear; year <= this.EndYear; year++)
                    {
                        if (modelValue.ContainsKey(year))
                        {
                            // SheetHeaders.Count + Blank Line + ColumnHeader Lines + Blank Line + 2
                            row = sectionRow + 1;

                            foreach (var state in vertex.States)
                            {
                                worksheet.WriteValue(row, col, modelValue[year][vertex.Key][state.Key], isText: true);
                                row++;
                            }
                        }

                        col++;
                    }

                    // col += (this.EndYear - this.StartYear) + 3;
                    col += 1;
                }

                sectionRow += this.Vertices.Max(vertex => vertex.States.Count) + 1;
            }
            
        }
    }
}