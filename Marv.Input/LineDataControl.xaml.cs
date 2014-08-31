using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

        public static readonly DependencyProperty EndYearProperty =
            DependencyProperty.Register("EndYear", typeof (int), typeof (LineDataControl), new PropertyMetadata(2010, ChangedEndYear));

        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register("FileName", typeof (string), typeof (LineDataControl), new PropertyMetadata(null));

        public static readonly DependencyProperty LineDataProperty =
            DependencyProperty.Register("LineData", typeof (Dict<string, int, string, VertexData>), typeof (LineDataControl), new PropertyMetadata(null, ChangedLineData));

        public static readonly DependencyProperty StartYearProperty =
            DependencyProperty.Register("StartYear", typeof (int), typeof (LineDataControl), new PropertyMetadata(2010, ChangedStartYear));

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

        public int EndYear
        {
            get
            {
                return (int) GetValue(EndYearProperty);
            }

            set
            {
                SetValue(EndYearProperty, value);
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

        public Dict<string, int, string, VertexData> LineData
        {
            get
            {
                return (Dict<string, int, string, VertexData>) GetValue(LineDataProperty);
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

        public int StartYear
        {
            get
            {
                return (int) GetValue(StartYearProperty);
            }
            set
            {
                SetValue(StartYearProperty, value);
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

        private static void ChangedEndYear(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LineDataControl;

            if (control.EndYear < control.StartYear)
            {
                control.EndYear = control.StartYear;
            }

            var newEndYear = (int) e.NewValue;
            var oldEndYear = (int) e.OldValue;

            control.UpdateRows(control.StartYear, newEndYear, control.StartYear, oldEndYear);
        }

        private static void ChangedLineData(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LineDataControl;

            control.InitializeRows();

            control.LineData.CollectionChanged += control.LineData_CollectionChanged;
        }

        private static void ChangedStartYear(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LineDataControl;

            if (control.StartYear > control.EndYear)
            {
                control.EndYear = control.StartYear;
            }

            var newStartYear = (int) e.NewValue;
            var oldStartYear = (int) e.OldValue;

            control.UpdateRows(newStartYear, control.EndYear, oldStartYear, control.EndYear);
        }

        private static void ChangedVertex(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LineDataControl;
            control.InitializeRows();
        }

        private void GridView_AutoGeneratingColumn(object sender, GridViewAutoGeneratingColumnEventArgs e)
        {
            e.Column.CellTemplateSelector = (CellTemplateSelector) this.FindResource("CellTemplateSelector");
        }

        private void GridView_CellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {
            var cellModel = e.Cell.ToModel();

            if (cellModel.IsColumnSectionId)
            {
                cellModel.Data = e.NewData as string;
                this.LineData.ChangeKey(e.OldData as string, e.NewData as string);
            }
            else
            {
                var distribution = this.Vertex.States.Parse(e.NewData as string).ToArray();

                var vertexData = this.LineData[cellModel.SectionId][cellModel.Year][this.Vertex.Key];
                vertexData.Evidence = distribution;
                vertexData.String = e.NewData as string;

                cellModel.Data = vertexData;

                this.CurrentGraphData = this.LineData[cellModel.SectionId][cellModel.Year];
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
            this.CurrentGraphData = this.LineData[cellModel.SectionId][cellModel.Year];
        }

        private void InitializeRows()
        {
            this.UpdateRows(this.StartYear, this.EndYear, this.StartYear, this.EndYear);
        }

        private void InsertSection(string sectionId, int index = -1)
        {
            if (this.LineData == null || this.Vertex == null)
            {
                return;
            }

            var row = new Dynamic();

            row[CellModel.SectionIdHeader] = sectionId;

            for (var year = this.StartYear; year <= this.EndYear; year++)
            {
                row[year.ToString()] = this.LineData[sectionId][year][this.Vertex.Key];
            }

            if (index == -1)
            {
                index = this.Rows.Count;
            }

            this.Rows.Insert(index, row);
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

        void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "MARV Input File|*.input",
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                this.FileName = dialog.FileName;

                this.LineData = Utils.ReadJson<Dict<string, int, string, VertexData>>(this.FileName);

                var newStartYear = this.LineData.Values.Min(sectionData => sectionData.Keys.Min());
                var newEndYear = this.LineData.Values.Max(sectionData => sectionData.Keys.Max());

                this.UpdateRows(newStartYear, newEndYear, newStartYear, newEndYear);
            }
        }

        private void LineData_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    var kvp = item as Kvp<string, Dict<int, string, VertexData>>;

                    var row = this.Rows.FirstOrDefault(r => r[CellModel.SectionIdHeader] == kvp.Key);

                    this.Rows.Remove(row);
                }
            }

            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    var kvp = item as Kvp<string, Dict<int, string, VertexData>>;
                    var index = this.LineData.IndexOf(kvp);
                    this.InsertSection(kvp.Key, index);
                }
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

        private void UpdateRows(int newStartYear, int newEndYear, int oldStartYear, int oldEndYear)
        {
            this.Rows = new ObservableCollection<Dynamic>();

            if (this.LineData == null || this.Vertex == null)
            {
                return;
            }

            var endYear = Utils.Max(newEndYear, oldEndYear);
            var startYear = Utils.Min(newStartYear, oldStartYear);

            foreach (var sectionId in this.LineData.Keys)
            {
                var row = new Dynamic();
                row[CellModel.SectionIdHeader] = sectionId;

                for (var year = startYear; year <= endYear; year++)
                {
                    if (year < newStartYear || newEndYear < year)
                    {
                        this.LineData[sectionId][year] = null;
                    }
                    else
                    {
                        row[year.ToString()] = this.LineData[sectionId][year][this.Vertex.Key];
                    }
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