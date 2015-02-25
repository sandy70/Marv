using System;
using System.Collections.Generic;
using System.Linq;
using Marv.Common;
using Microsoft.Office.Interop.Excel;

namespace Marv.ExcelNew
{
    public class SheetModel
    {
        private List<string> columnHeaders = new List<string>();
        private Dict<string, int, string, VertexEvidence> lineEvidence = new Dict<string, int, string, VertexEvidence>();
        private Dict<string, int, string, double[]> lineValue = new Dict<string, int, string, double[]>();
        private Dict<int, string, string, double> modelEvidence = new Dict<int, string, string, double>();
        private Dictionary<string, object> sheetHeaders = new Dictionary<string, object>();
        private IEnumerable<Vertex> vertices;

        public List<string> ColumnHeaders
        {
            get { return this.columnHeaders; }
            set { this.columnHeaders = value; }
        }

        public int EndYear { get; set; }

        public Graph Graph { get; set; }

        public Dict<string, int, string, VertexEvidence> LineEvidence
        {
            get { return this.lineEvidence; }
            set { this.lineEvidence = value; }
        }

        public Dict<string, int, string, double[]> LineValue
        {
            get { return this.lineValue; }
            set { this.lineValue = value; }
        }

        public Dict<int, string, string, double> ModelEvidence
        {
            get { return this.modelEvidence; }
            set { this.modelEvidence = value; }
        }

        public Dict<int, string, string, double> ModelValue { get; set; }

        public Dictionary<string, object> SheetHeaders
        {
            get { return this.sheetHeaders; }
            set { this.sheetHeaders = value; }
        }

        public int StartYear { get; set; }

        public IEnumerable<Vertex> Vertices
        {
            get
            {
                if (this.vertices == null)
                {
                    return this.Graph.Vertices;
                }

                return this.vertices;
            }

            set { this.vertices = value; }
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
            var currentFind = worksheet.Cells.Find("Belief");

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

                // Get sectionId
                var sectionId = worksheet.ReadText(row, 1);

                if (!sheetModel.LineEvidence.ContainsKey(sectionId))
                {
                    sheetModel.LineEvidence[sectionId] = new Dict<int, string, VertexEvidence>();
                }

                // Get vertexKey
                var vertexKey = worksheet.ReadText(sheetModel.SheetHeaders.Count + 2, col);
                var vertex = sheetModel.Graph.Vertices[vertexKey];

                col++;

                for (var year = sheetModel.StartYear; year <= sheetModel.EndYear; year++)
                {
                    if (sheetModel.LineEvidence[sectionId].ContainsKey(year))
                    {
                        sheetModel.LineEvidence[sectionId][year] = new Dict<string, VertexEvidence>();
                    }

                    value = worksheet.Read(row, col);

                    if (value != null)
                    {
                        var evidence = vertex.States.ParseEvidenceString(value.ToString());
                        sheetModel.LineEvidence[sectionId][year][vertexKey] = evidence;
                    }
                    else
                    {
                        var vertexEvidence = new VertexEvidence
                        {
                            Value = new double[vertex.States.Count]
                        };

                        var isEvidenceNull = true;

                        foreach (var state in vertex.States)
                        {
                            var i = vertex.States.IndexOf(state);

                            var stateValue = worksheet.Read(row + i + 1, col);

                            if (stateValue == null)
                            {
                                vertexEvidence.Value[i] = 0;
                            }
                            else
                            {
                                isEvidenceNull = false;
                                vertexEvidence.Value[i] = Convert.ToDouble(stateValue);
                            }
                        }

                        if (!isEvidenceNull)
                        {
                            sheetModel.LineEvidence[sectionId][year][vertexKey] = vertexEvidence;
                        }
                    }

                    col++;
                }

                currentFind = worksheet.Cells.FindNext(currentFind);
            }

            return sheetModel;
        }

        public void Run()
        {
            foreach (var sectionId in this.LineEvidence.Keys)
            {
                this.LineValue[sectionId] = this.Graph.Network.Run(this.LineEvidence[sectionId]);
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

                // col++;
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

                    worksheet.WriteValue(row, col, "Belief");
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
                                var iState = vertex.States.IndexOf(state);
                                worksheet.WriteValue(row, col, modelValue[year][vertex.Key][iState], isText: true);
                                row++;
                            }
                        }

                        col++;
                    }

                    // col += (this.EndYear - this.StartYear) + 3;
                    // col += 1;
                }

                sectionRow += this.Vertices.Max(vertex => vertex.States.Count) + 1;
            }
        }
    }
}