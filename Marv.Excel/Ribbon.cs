using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;
using Microsoft.Office.Interop.Excel;
using Smile;

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

            var network = new Network();
            network.ReadFile(dialog.FileName);

            var nodeIds = network.GetAllNodeIds();
            var rowIndex = 1;

            foreach(var nodeId in nodeIds)
            {
                Range range = worksheet.Cells.get_Item(rowIndex++, 1);
                range.Value2 = nodeId;
            }
        }
    }
}
