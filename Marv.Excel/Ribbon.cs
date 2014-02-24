using System.Collections.Generic;
using Marv.Common;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Tools.Ribbon;

namespace Marv.Excel
{
    public partial class Ribbon
    {
        private void Ribbon_Load(object sender, RibbonUIEventArgs e)
        {
            this.OpenFileButton.Click += OpenFileButton_Click;
            this.RunButton.Click += RunButton_Click;
        }

        private void RunButton_Click(object sender, RibbonControlEventArgs e)
        {
            var colIndex = 4;
            var graphEvidence = new Dictionary<string, IEvidence>();
            var worksheet = (Worksheet)Globals.ThisAddIn.Application.ActiveSheet;

            var fileName = (string)worksheet.Cells[1, 2].Value2;
            var graph = Graph.Read(fileName);

            var cell = worksheet.Cells[2, colIndex];

            while (cell.Value2 != null)
            {
                var isEvidenceAvailable = true;
                var rowIndex = 6;
                var vertexKey = cell.Value2;
                var statesCount = graph.Vertices[vertexKey].States.Count;
                var evidence = new double[statesCount];

                try
                {
                    for (var i = 0; i < statesCount; i++)
                    {
                        evidence[i] = worksheet.Cells[rowIndex++, colIndex].Value2;
                    }
                }
                catch (RuntimeBinderException)
                {
                    isEvidenceAvailable = false;
                }

                if (isEvidenceAvailable)
                {
                    graphEvidence[vertexKey] = new SoftEvidence
                    {
                        Evidence = evidence,
                    };
                }

                colIndex += 2;
                cell = worksheet.Cells[2, colIndex];
            }

            var graphValue = graph.Run(graphEvidence);
            this.WriteGraphValue(graphValue, "Output");
        }

        private void WriteGraphValue(Dictionary<string, string, double> graphValue, string worksheetName)
        {
            var worksheet = (Worksheet) Globals.ThisAddIn.Application.ActiveWorkbook.Worksheets.Add();
            worksheet.Name = worksheetName;

            var colIndex = 1;

            foreach (var vertexKey in graphValue.Keys)
            {
                var rowIndex = 1;
                var vertexValue = graphValue[vertexKey];

                worksheet.Cells[rowIndex++, colIndex] = vertexKey;

                foreach (var stateKey in vertexValue.Keys)
                {
                    worksheet.Cells[rowIndex, colIndex] = stateKey;
                    worksheet.Cells[rowIndex++, colIndex + 1] = vertexValue[stateKey];
                }

                colIndex += 2;
            }
        }

        private void OpenFileButton_Click(object sender, RibbonControlEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.ShowDialog();

            var worksheet = (Worksheet)Globals.ThisAddIn.Application.ActiveSheet;

            var rowIndex = 1;

            // Generate the static part
            worksheet.Cells[rowIndex, 1] = "File Name";
            worksheet.Cells[rowIndex++, 2] = dialog.FileName;

            worksheet.Cells[rowIndex++, 1] = "Name";
            worksheet.Cells[rowIndex++, 1] = "Units";
            worksheet.Cells[rowIndex++, 1] = "Description";
            worksheet.Cells[rowIndex++, 1] = "Value";

            var evidenceStartRowIndex = rowIndex;

            var graph = Graph.Read(dialog.FileName);

            var selectVerticesWindow = new SelectVerticesWindow
            {
                Vertices = graph.Vertices
            };

            selectVerticesWindow.ShowDialog();

            var selectedItems = selectVerticesWindow.VerticesListBox.SelectedItems;

            var colIndex = 4;

            foreach (var item in selectedItems)
            {
                var vertex = item as Vertex;

                // Set the header cells for this vertex
                worksheet.Cells[2, colIndex] = vertex.Key;
                worksheet.Cells[3, colIndex] = vertex.Units;
                worksheet.Cells[4, colIndex] = vertex.Description;

                rowIndex = evidenceStartRowIndex;

                // Set the state cells for this vertex
                foreach (var state in vertex.States)
                {
                    // Prefixing with ' makes sure the value is formatted as text
                    worksheet.Cells[rowIndex++, colIndex - 1] = "'" + state.Key;
                }

                colIndex += 2;
            }
        }
    }
}