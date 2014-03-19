using System;
using System.Collections.Generic;
using System.Windows;
using AddinExpress.XL;
using Marv.Common.Graph;
using Microsoft.Office.Interop.Excel;

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

        private void WriteHeader()
        {
            var fileName = Marv_Excel.AddinModule.CurrentInstance.FileName;
            var nYears = this.vertexSelectionControl.nYears;
            var selectedVertices = this.vertexSelectionControl.SelectedVertices;
            var worksheet = (Worksheet) Marv_Excel.AddinModule.ExcelApp.ActiveSheet;

            var row = 1;
            var col = 1;

            ((Range) worksheet.Cells[row, col]).Font.Bold = true;
            worksheet.Cells[row, col++] = "Network File";
            worksheet.Cells[row++, col] = fileName;

            col = 1;
            row++;

            worksheet.WriteValue(row, col, "Section Name", true);
            col++;

            worksheet.WriteValue(row, col, "Latitude", true);
            col++;

            worksheet.WriteValue(row, col, "Longitude", true);
            col += 2;

            

            if (selectedVertices != null)
            {
                foreach (var vertex in selectedVertices)
                {
                    worksheet.WriteValue(row, col, vertex.Key, true);
                    col++;

                    for (var year = 0; year < nYears; year++)
                    {
                        worksheet.WriteValue(row, col, year, isText: true);
                        col++;
                    }

                    col++;
                }
            }
        }

        private void WriteVertexSkeleton(int row, int col, Vertex vertex)
        {
            var worksheet = (Worksheet)Marv_Excel.AddinModule.ExcelApp.ActiveSheet;

            worksheet.WriteValue(row, col, "Value");
            row++;

            foreach (var state in vertex.States)
            {
                worksheet.WriteValue(row, col, state.Key, isText: true);
                row++;
            }
        }

        private void vertexSelectionControl_DoneButtonClicked(object sender, RoutedEventArgs e)
        {
            var nYears = this.vertexSelectionControl.nYears;
            var selectedVertices = this.vertexSelectionControl.SelectedVertices;

            this.WriteHeader();

            var col = 5;

            foreach (var vertex in selectedVertices)
            {
                const int row = 5;
                this.WriteVertexSkeleton(row, col, vertex);
                col += nYears;
                col++;
            }

            this.Hide();
        }
    }
}