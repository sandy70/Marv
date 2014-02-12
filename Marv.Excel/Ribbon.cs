using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;
using System.Windows.Forms;

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
            var dialog = new OpenFileDialog();
            var result = dialog.ShowDialog();

            if(result != null)
            {
                // var graph = Graph.Read(dialog.FileName);
            }
        }
    }
}
