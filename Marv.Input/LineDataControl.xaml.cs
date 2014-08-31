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

        private void GridView_AutoGeneratingColumn(object sender, GridViewAutoGeneratingColumnEventArgs e)
        {
            e.Column.CellTemplateSelector = (CellTemplateSelector) this.FindResource("CellTemplateSelector");
            e.Column.Width = 64;
        }

        private void GridView_CellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {
            var cellModel = e.Cell.ToModel();

            if (cellModel.IsColumnSectionId)
            {
                cellModel.Data = e.NewData as string;
                this.LineData.Sections.ChangeKey(e.OldData as string, e.NewData as string);
            }
            else
            {
                var distribution = this.Vertex.States.Parse(e.NewData as string).ToArray();

                var vertexData = this.LineData.Sections[cellModel.SectionId][cellModel.Year][this.Vertex.Key];
                vertexData.Evidence = distribution;
                vertexData.String = e.NewData as string;

                cellModel.Data = vertexData;

                this.CurrentGraphData = this.LineData.Sections[cellModel.SectionId][cellModel.Year];
            }

            this.RaiseCellChanged(cellModel);
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

        public event EventHandler<CellModel> CellChanged;
    }
}