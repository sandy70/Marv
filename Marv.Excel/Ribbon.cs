using Marv.Common;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Tools.Ribbon;

namespace Marv.Excel
{
    public partial class Ribbon
    {
        private void Ribbon_Load(object sender, RibbonUIEventArgs e)
        {
            this.OpenFileButton.Click += OpenFileButton_Click;
        }

        private void OpenFileButton_Click(object sender, RibbonControlEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.ShowDialog();

            var worksheet = (Worksheet)Globals.ThisAddIn.Application.ActiveSheet;

            // Generate the static part
            worksheet.Cells[1, 1] = "Name";
            worksheet.Cells[2, 1] = "Units";
            worksheet.Cells[3, 1] = "Description";
            worksheet.Cells[4, 1] = "Value";

            var graph = Graph.Read(dialog.FileName);

            var selectVerticesWindow = new SelectVerticesWindow();
            selectVerticesWindow.Vertices = graph.Vertices;

            selectVerticesWindow.ShowDialog();

            var selectedItems = selectVerticesWindow.VerticesListBox.SelectedItems;

            var colIndex = 4;

            foreach (var item in selectedItems)
            {
                var vertex = item as Vertex;

                // Set the header cells for this vertex
                worksheet.Cells[1, colIndex] = vertex.Key;
                worksheet.Cells[2, colIndex] = vertex.Units;
                worksheet.Cells[3, colIndex] = vertex.Description;

                var rowIndex = 5;
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