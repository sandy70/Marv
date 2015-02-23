using System.Windows.Forms;
using Microsoft.Office.Tools.Ribbon;

namespace Marv.ExcelNew
{
    public partial class Ribbon
    {
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
    }
}