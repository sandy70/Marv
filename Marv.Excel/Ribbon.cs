using System;
using System.Windows.Forms;
using Marv.Common;
using Microsoft.Office.Tools.Ribbon;

namespace Marv.ExcelNew
{
    public partial class Ribbon
    {
        public VertexListBox VertexListBox
        {
            get
            {
                return (Globals.ThisAddIn.CustomTaskPane.Control as WpfHost).ElementHost.Child as VertexListBox;
            }
        }

        private void OpenButton_Click(object sender, RibbonControlEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "NetworkFile (*.net)|*.net",
                Multiselect = false
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var graph = Graph.Read(dialog.FileName);
            }
        }

        private void Ribbon_Load(object sender, RibbonUIEventArgs e) {}

        private void ToggleMarvPaneButton_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.CustomTaskPane.Visible = this.ToggleMarvPaneButton.Checked;
        }

        private void ModelRunButton_Click(object sender, RibbonControlEventArgs e)
        {
            this.VertexListBox.OutputSheet.Cells.Clear();

            var sheetModel = SheetModel.Read(this.VertexListBox.InputSheet);
            sheetModel.Run();
            sheetModel.Write(this.VertexListBox.OutputSheet);

            this.VertexListBox.OutputSheet.Activate();
        }
    }
}