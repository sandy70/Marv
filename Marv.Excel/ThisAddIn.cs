using Microsoft.Office.Interop.Excel;

namespace Marv.Excel
{
    public partial class ThisAddIn
    {
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            this.Application.WorkbookBeforeSave += Application_WorkbookBeforeSave;
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        private void Application_WorkbookBeforeSave(Microsoft.Office.Interop.Excel.Workbook Wb, bool SaveAsUI, ref bool Cancel)
        {
            Worksheet activeWorksheet = ((Worksheet)Application.ActiveSheet);
            Range firstRow = activeWorksheet.get_Range("A1");
            firstRow.EntireRow.Insert(XlInsertShiftDirection.xlShiftDown);
            Range newFirstRow = activeWorksheet.get_Range("A1");
            newFirstRow.Value2 = "This text was added by using code";
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion VSTO generated code
    }
}