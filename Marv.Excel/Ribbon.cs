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

            var graph = Graph.Read(dialog.FileName);

            var rowIndex = 1;

            foreach(var vertex in graph.Vertices)
            {
                Range range = worksheet.Cells.get_Item(rowIndex, 1);
                range.Value2 = vertex.Name;
                worksheet.Cells.get_Item(rowIndex, 2).Value2 = vertex.Key;
                worksheet.Cells.get_Item(rowIndex++, 3).Value2 = vertex.Units;
            }
        }
    }
}
