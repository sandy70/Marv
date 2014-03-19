using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using AddinExpress.XL;
using Marv.Common.Graph;

namespace Marv_Excel
{
    public partial class TaskPane : ADXExcelTaskPane
    {
        private bool isHiddenBefore;

        public TaskPane()
        {
            InitializeComponent();
            this.Activated += TaskPane_Activated;

            this.vertexSelectionControl.DoneButtonClicked += vertexSelectionControl_DoneButtonClicked;
        }

        public void SetVertices(IEnumerable<Vertex> vertices)
        {
            this.vertexSelectionControl.Vertices = vertices;
        }

        private void TaskPane_Activated(object sender, EventArgs e)
        {
            if (!isHiddenBefore)
            {
                this.Hide();
                isHiddenBefore = true;
            }
        }

        private void TaskPane_SizeChanged(object sender, EventArgs e)
        {
            this.elementHost.Height = this.Height - 20;
            this.elementHost.Width = this.Width - 20;
        }

        private void vertexSelectionControl_DoneButtonClicked(object sender, RoutedEventArgs e)
        {
            var fileName = Marv_Excel.AddinModule.CurrentInstance.FileName;
            var nYears = Marv_Excel.AddinModule.CurrentInstance.nYears = this.vertexSelectionControl.nYears;
            var selectedVertices = this.vertexSelectionControl.SelectedVertices.ToList();
            var workbook = Marv_Excel.AddinModule.ExcelApp.ActiveWorkbook;
            var worksheet = workbook.GetWorksheetOrNew("Input");

            worksheet.WriteHeader(fileName, selectedVertices, nYears);
            worksheet.WriteVertexSkeletons(selectedVertices, nYears);

            this.Hide();
        }
    }
}