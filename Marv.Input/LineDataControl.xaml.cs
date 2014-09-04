using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Marv.Common;
using Marv.Common.Graph;
using Microsoft.Win32;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using System.Collections.Generic;

namespace Marv.Input
{
    public partial class LineDataControl : INotifyPropertyChanged
    {
        public static readonly DependencyProperty CurrentGraphDataProperty =
            DependencyProperty.Register("CurrentGraphData", typeof (Dict<string, VertexData>), typeof (LineDataControl), new PropertyMetadata(null));

        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register("FileName", typeof (string), typeof (LineDataControl), new PropertyMetadata(null));

        public static readonly DependencyProperty LineDataProperty =
            DependencyProperty.Register("LineData", typeof (LineData), typeof (LineDataControl), new PropertyMetadata(null, ChangedLineData));

        public static readonly DependencyProperty SectionsToAddCountProperty =
            DependencyProperty.Register("SectionsToAddCount", typeof (int), typeof (LineDataControl), new PropertyMetadata(1));

        public static readonly DependencyProperty VertexProperty =
            DependencyProperty.Register("Vertex", typeof (Vertex), typeof (LineDataControl), new PropertyMetadata(null, ChangedVertex));

        private ObservableCollection<Dynamic> rows;

        public Dict<string, VertexData> CurrentGraphData
        {
            get
            {
                return (Dict<string, VertexData>) GetValue(CurrentGraphDataProperty);
            }
            set
            {
                SetValue(CurrentGraphDataProperty, value);
            }
        }

        public string FileName
        {
            get
            {
                return (string) GetValue(FileNameProperty);
            }
            set
            {
                SetValue(FileNameProperty, value);
            }
        }

        public LineData LineData
        {
            get
            {
                return (LineData) GetValue(LineDataProperty);
            }

            set
            {
                SetValue(LineDataProperty, value);
            }
        }

        public ObservableCollection<Dynamic> Rows
        {
            get
            {
                return this.rows;
            }

            set
            {
                if (value != this.rows)
                {
                    this.rows = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public int SectionsToAddCount
        {
            get
            {
                return (int) GetValue(SectionsToAddCountProperty);
            }
            set
            {
                SetValue(SectionsToAddCountProperty, value);
            }
        }

        public Vertex Vertex
        {
            get
            {
                return (Vertex) GetValue(VertexProperty);
            }
            set
            {
                SetValue(VertexProperty, value);
            }
        }

        public LineDataControl()
        {
            InitializeComponent();

            this.Loaded += LineDataControl_Loaded;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private static void ChangedLineData(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LineDataControl;

            control.LineData.DataChanged += control.LineData_DataChanged;

            control.UpdateRows();
        }

        private static void ChangedVertex(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LineDataControl;
            control.UpdateRows();
        }

        private void AddSectionsButton_Click(object sender, RoutedEventArgs e)
        {
            var nSection = 1;

            var keys = Enumerable.Range(0, this.SectionsToAddCount).Select(i =>
            {
                var sectionId = "Section " + nSection;

                while (this.LineData.Sections.ContainsKey(sectionId))
                {
                    nSection++;
                    sectionId = "Section " + nSection;
                }

                return sectionId;
            });

            this.LineData.AddSections(keys);
        }

        private void GridView_AutoGeneratingColumn(object sender, GridViewAutoGeneratingColumnEventArgs e)
        {
            e.Column.CellTemplateSelector = (CellTemplateSelector) this.FindResource("CellTemplateSelector");
        }

        private void GridView_CellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {
            var cellModel = e.Cell.ToModel();
            var oldString = e.OldData as string;
            var newString = e.NewData as string;

            this.SetCell(cellModel, newString, oldString);
        }

        private void GridView_CellValidating(object sender, GridViewCellValidatingEventArgs e)
        {
            if (e.Cell.ToModel().IsColumnSectionId)
            {
                return;
            }

            var vertexEvidenceType = this.Vertex.GetEvidenceType(e.NewValue as string);

            if (vertexEvidenceType == VertexEvidenceType.Invalid)
            {
                e.IsValid = false;
                e.ErrorMessage = "Not a correct value or range of values. Press ESC to cancel.";
            }
        }

        private void GridView_CurrentCellChanged(object sender, GridViewCurrentCellChangedEventArgs e)
        {
            if (e.NewCell == null || this.LineData == null)
            {
                return;
            }

            var cellModel = e.NewCell.ToModel();

            if (cellModel.IsColumnSectionId)
            {
                this.GridView.SelectionUnit = GridViewSelectionUnit.FullRow;
                return;
            }

            this.GridView.SelectionUnit = GridViewSelectionUnit.Cell;
            this.CurrentGraphData = this.LineData.Sections[cellModel.SectionId][cellModel.Year];
        }

        private void LineDataControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.GridView.AutoGeneratingColumn -= GridView_AutoGeneratingColumn;
            this.GridView.AutoGeneratingColumn += GridView_AutoGeneratingColumn;

            this.GridView.CellEditEnded -= GridView_CellEditEnded;
            this.GridView.CellEditEnded += GridView_CellEditEnded;

            this.GridView.CellValidating -= GridView_CellValidating;
            this.GridView.CellValidating += GridView_CellValidating;

            this.GridView.CurrentCellChanged -= GridView_CurrentCellChanged;
            this.GridView.CurrentCellChanged += GridView_CurrentCellChanged;

            this.AddSectionsButton.Click -= AddSectionsButton_Click;
            this.AddSectionsButton.Click += AddSectionsButton_Click;

            this.OpenButton.Click -= OpenButton_Click;
            this.OpenButton.Click += OpenButton_Click;

            this.SaveButton.Click -= SaveButton_Click;
            this.SaveButton.Click += SaveButton_Click;
        }

        private void LineData_DataChanged(object sender, EventArgs e)
        {
            this.UpdateRows();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "MARV Input File|*.input",
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                this.FileName = dialog.FileName;
                this.LineData = Utils.ReadJson<LineData>(this.FileName);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.FileName == null)
            {
                var dialog = new SaveFileDialog
                {
                    Filter = "MARV Input Files|*.input",
                };

                var result = dialog.ShowDialog();

                if (result == true)
                {
                    this.FileName = dialog.FileName;
                }
            }

            if (this.FileName != null)
            {
                this.LineData.WriteJson(this.FileName);
            }
        }

        private void SetCell(CellModel cellModel, string newString, string oldString = null)
        {
            if (cellModel.IsColumnSectionId)
            {
                if (oldString == null)
                {
                    this.LineData.Sections[newString] = new Dict<int, string, VertexData>();
                }
                else
                {
                    this.LineData.Sections.ChangeKey(oldString, newString);
                }

                cellModel.Data = newString;
            }
            else
            {
                var distribution = this.Vertex.States.Parse(newString).ToArray();

                var vertexData = this.LineData.Sections[cellModel.SectionId][cellModel.Year][this.Vertex.Key];
                vertexData.Evidence = distribution;
                vertexData.String = newString;

                cellModel.Data = vertexData;

                this.CurrentGraphData = this.LineData.Sections[cellModel.SectionId][cellModel.Year];
            }

            this.RaiseCellChanged(cellModel);
        }

        private void UpdateRows()
        {
            if (this.LineData == null || this.Vertex == null)
            {
                return;
            }

            this.Rows = new ObservableCollection<Dynamic>();

            foreach (var sectionId in this.LineData.Sections.Keys)
            {
                var row = new Dynamic();
                row[CellModel.SectionIdHeader] = sectionId;

                for (var year = this.LineData.StartYear; year <= this.LineData.EndYear; year++)
                {
                    row[year.ToString()] = this.LineData.Sections[sectionId][year][this.Vertex.Key];
                }

                this.Rows.Add(row);
            }
        }

        public void RaiseCellChanged(CellModel cellModel)
        {
            if (this.CellChanged != null)
            {
                this.CellChanged(this, cellModel);
            }
        }

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null && propertyName != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void SetSelectedCells(VertexData vertexData)
        {
            foreach (var cell in this.GridView.SelectedCells)
            {
                var cellModel = cell.ToModel();
                this.SetCell(cellModel, vertexData.String);
            }
        }

        public event EventHandler<CellModel> CellChanged;
    }
}