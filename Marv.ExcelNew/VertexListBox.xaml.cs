using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using Telerik.Windows.Controls;

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

        public int EndYear
        {
            get { return (int) GetValue(EndYearProperty); }
            set { SetValue(EndYearProperty, value); }
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

        public VertexListBox()
        {
            InitializeComponent();

            this.Loaded += VerticesSelectionControl_Loaded;
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            var lineData = new LineData();
            lineData.StartYear = this.StartYear;
            lineData.EndYear = this.EndYear;

            var sectionBelief = new Dict<int, string, double[]>();

            for (var year = this.StartYear; year <= this.EndYear; year++)
            {
                foreach (var vertex in this.SelectedVertices)
                {
                    sectionBelief[year][vertex.Key] = new double[vertex.States.Count];
                }
            }

            lineData.SetBelief("Section 1", sectionBelief);

            this.Write(lineData);
        }

        private void Write(LineData lineData)
        {
            var worksheet = Globals.ThisAddIn.Application.ActiveSheet as Worksheet;

            worksheet.Write(1, 1, "Hello world");
        }

        private void OpenNetworkButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "NetworkFile (*.net)|*.net",
                Multiselect = false
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.Vertices = Graph.Read(dialog.FileName).Vertices;
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
    }
}