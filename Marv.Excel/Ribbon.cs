using System.Collections.Generic;
using System.Windows.Forms;
using Marv.Common.Graph;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Tools.Ribbon;

namespace Marv.Excel
{
    public partial class Ribbon
    {
        private void OpenFileButton_Click(object sender, RibbonControlEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.ShowDialog();

            var worksheet = (Worksheet) Globals.ThisAddIn.Application.ActiveSheet;

            var graph = Graph.Read(dialog.FileName);

            var selectVerticesWindow = new SelectVerticesWindow
            {
                Vertices = graph.Vertices
            };

            selectVerticesWindow.ShowDialog();

            var selectedVertices = new List<Vertex>();

            foreach (var item in selectVerticesWindow.VerticesListBox.SelectedItems)
            {
                selectedVertices.Add(item as Vertex);
            }

            var fileName = dialog.FileName;

            worksheet.WriteVertexValues(selectedVertices, graph.Belief, fileName, false);
        }

        private void Ribbon_Load(object sender, RibbonUIEventArgs e)
        {
            this.OpenFileButton.Click += OpenFileButton_Click;
            this.RunButton.Click += RunButton_Click;
        }

        private void RunButton_Click(object sender, RibbonControlEventArgs e)
        {
            var colIndex = 4;
            var graphEvidence = new Dictionary<string, IEvidence>();
            var workbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            var worksheet = (Worksheet) Globals.ThisAddIn.Application.ActiveSheet;

            var fileName = (string) worksheet.Cells[1, 2].Value2;
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

            var outputWorksheet = workbook.GetWorksheetOrNew("Output");
            outputWorksheet.WriteVertexValues(graph.Vertices, graphValue, fileName);
            outputWorksheet.Select();
        }
    }
}