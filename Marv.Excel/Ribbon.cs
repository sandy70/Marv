using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;
using Microsoft.Office.Interop.Excel;
using Smile;
using Marv.Common;

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
            var result = dialog.ShowDialog();

            var worksheet = (Worksheet)Globals.ThisAddIn.Application.ActiveSheet;

            // Generate the static part
            Ribbon.SetCellValue(worksheet, 1, 1, "Name");
            Ribbon.SetCellValue(worksheet, 2, 1, "Units");
            Ribbon.SetCellValue(worksheet, 3, 1, "Description");
            Ribbon.SetCellValue(worksheet, 4, 1, "Value");

            var graph = Graph.Read(dialog.FileName);

            var colIndex = 4;

            foreach(var vertex in graph.Vertices)
            {
                // Set the header cells for this vertex
                worksheet.SetCellValue(1, colIndex, vertex.Key);
                worksheet.SetCellValue(2, colIndex, vertex.Units);
                worksheet.SetCellValue(3, colIndex, vertex.Description);

                var rowIndex = 5;
                // Set the state cells for this vertex
                foreach(var state in vertex.States)
                {
                    worksheet.SetCellValue(rowIndex++, colIndex - 1, state.Key);
                }

                colIndex += 2;
            }
        }

        private static void SetCellValue(Worksheet worksheet, int rowIndex, int colIndex, string value)
        {
            var range = (Range)worksheet.Cells.get_Item(rowIndex, colIndex);
            range.Value2 = value;
        }
    }
}
