using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using Telerik.Windows.Controls;
using Application = Microsoft.Office.Interop.Excel.Application;

namespace Marv.ExcelNew
{
    public partial class VertexListBox
    {
        public static readonly DependencyProperty EndYearProperty =
            DependencyProperty.Register("EndYear", typeof (int), typeof (VertexListBox), new PropertyMetadata(2000));

        public static readonly DependencyProperty StartYearProperty =
            DependencyProperty.Register("StartYear", typeof (int), typeof (VertexListBox), new PropertyMetadata(2000));

        public static readonly DependencyProperty VerticesProperty =
            DependencyProperty.Register("Vertices", typeof (IEnumerable<Vertex>), typeof (VertexListBox), new PropertyMetadata(null));

        private string fileName;

        public Application Application
        {
            get { return Globals.ThisAddIn.Application; }
        }

        public int EndYear
        {
            get { return (int) GetValue(EndYearProperty); }
            set { SetValue(EndYearProperty, value); }
        }

        public Worksheet InputSheet
        {
            get { return this.Workbook.GetWorksheetOrNew("Input"); }
        }

        public Worksheet OutputSheet
        {
            get { return this.Workbook.GetWorksheetOrNew("Output"); }
        }

        public IEnumerable<Vertex> SelectedVertices
        {
            get { return this.VerticesListBox.SelectedItems.Cast<Vertex>(); }
        }

        public int StartYear
        {
            get { return (int) GetValue(StartYearProperty); }
            set { SetValue(StartYearProperty, value); }
        }

        public IEnumerable<Vertex> Vertices
        {
            get { return (IEnumerable<Vertex>) GetValue(VerticesProperty); }
            set { SetValue(VerticesProperty, value); }
        }

        public Workbook Workbook
        {
            get
            {
                if (this.Application.ActiveWorkbook == null)
                {
                    this.Application.Workbooks.Add();
                }

                return this.Application.ActiveWorkbook;
            }
        }

        public VertexListBox()
        {
            InitializeComponent();

            this.Loaded += VerticesSelectionControl_Loaded;
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            if (!this.SelectedVertices.Any())
            {
                MessageBox.Show("Select at least 1 node.", "Error!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            this.InputSheet.Cells.Clear();
            this.InputSheet.Cells.NumberFormat = "@";

            var sheetModel = new SheetModel
            {
                SheetHeaders = new Dictionary<string, object>
                {
                    { "Network File", this.fileName },
                    { "Start Year", this.StartYear },
                    { "End Year", this.EndYear }
                },
                ColumnHeaders = new List<string>
                {
                    "Section Key",
                    "Latitude",
                    "Longitude"
                },
                StartYear = this.StartYear,
                EndYear = this.EndYear,
                Vertices = this.SelectedVertices
            };

            sheetModel.LineValue["One"] = new Dict<int, string, double[]>();
            sheetModel.Write(this.InputSheet);
        }

        private void OpenNetworkButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "NetworkFile (*.net)|*.net",
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                this.Vertices = Graph.Read(this.fileName = dialog.FileName).Vertices;
            }
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            this.VerticesListBox.SelectAll();
        }

        private void SelectNoneButton_Click(object sender, RoutedEventArgs e)
        {
            this.VerticesListBox.UnselectAll();
        }

        private void StartYearUpDown_ValueChanged(object sender, RadRangeBaseValueChangedEventArgs e)
        {
            if (this.EndYear < this.StartYear)
            {
                this.EndYear = this.StartYear;
            }
        }

        private void VerticesSelectionControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.SelectAllButton.Click += SelectAllButton_Click;
            this.SelectNoneButton.Click += SelectNoneButton_Click;
            this.StartYearUpDown.ValueChanged += StartYearUpDown_ValueChanged;
            this.DoneButton.Click += DoneButton_Click;
        }

        private void Write(LineData lineData)
        {
            var worksheet = Globals.ThisAddIn.Application.ActiveSheet as Worksheet;

            var row = 1;
            var col = 1;
            worksheet.Write(row, col, "Start Year", true);

            col++;
            worksheet.Write(row, col, lineData.StartYear);

            row++;
            col = 1;
            worksheet.Write(row, col, "End Year", true);

            col++;
            worksheet.Write(row, col, lineData.EndYear);

            row++;
            col = 1;
            worksheet.WriteRow(row, "");

            row++;
            worksheet.Write(row, col, "Section ID", true);

            col++;
            worksheet.WriteCol(col, "");

            foreach (var vertex in this.SelectedVertices)
            {
                col++;
                worksheet.Write(row, col, vertex.Key);

                for (var year = lineData.StartYear; year <= lineData.EndYear; year++)
                {
                    col++;
                    worksheet.Write(row, col, year);
                }

                col++;
                worksheet.WriteCol(col, "");
            }

            row++;
            col = 1;
            worksheet.WriteRow(row, "");

            foreach (var sectionId in lineData.SectionIds)
            {
                row++;
                worksheet.Write(row, col, sectionId);
            }
        }
    }
}