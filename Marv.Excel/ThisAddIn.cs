using System;
using Microsoft.Office.Tools;
using Office = Microsoft.Office.Core;

namespace Marv.ExcelNew
{
    public partial class ThisAddIn
    {
        public CustomTaskPane CustomTaskPane;
        private WpfHost wpfHost;

        private void ThisAddIn_Shutdown(object sender, EventArgs e) {}

        private void ThisAddIn_Startup(object sender, EventArgs e)
        {
            this.wpfHost = new WpfHost();
            this.CustomTaskPane = this.CustomTaskPanes.Add(this.wpfHost, "MARV - Multi-Analytic Risk Visualization");
            this.CustomTaskPane.Visible = true;
            this.CustomTaskPane.Width = 512;

            this.CustomTaskPane.VisibleChanged += customTaskPane_VisibleChanged;
        }

        private void customTaskPane_VisibleChanged(object sender, EventArgs e)
        {
            Globals.Ribbons.Ribbon.ToggleMarvPaneButton.Checked = this.CustomTaskPane.Visible;
        }

        #region VSTO generated code

        /// <summary>
        ///     Required method for Designer support - do not modify
        ///     the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += this.ThisAddIn_Startup;
            this.Shutdown += this.ThisAddIn_Shutdown;
        }

        #endregion
    }
}