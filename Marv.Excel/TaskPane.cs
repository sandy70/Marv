using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using AddinExpress.XL;
  
namespace Marv_Excel
{  
    public partial class TaskPane : AddinExpress.XL.ADXExcelTaskPane
    {
        public TaskPane()
        {
            InitializeComponent();
        }

        private void TaskPane_SizeChanged(object sender, EventArgs e)
        {
            this.elementHost.Height = this.Height - 20;
            this.elementHost.Width = this.Width - 20;
        }
    }
}
