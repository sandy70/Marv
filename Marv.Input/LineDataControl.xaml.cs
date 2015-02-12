using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Marv.Common;
using Microsoft.Win32;
using Telerik.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Marv.Input
{
    public partial class LineDataControl : INotifyPropertyChanged
    {
        public static readonly DependencyProperty CurrentGraphDataProperty =
            DependencyProperty.Register("CurrentGraphData", typeof (Dict<string, VertexEvidence>), typeof (LineDataControl), new PropertyMetadata(null));

        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register("FileName", typeof (string), typeof (LineDataControl), new PropertyMetadata(null));

        public static readonly DependencyProperty IsGridViewEnabledProperty =
            DependencyProperty.Register("IsGridViewEnabled", typeof (bool), typeof (LineDataControl), new PropertyMetadata(false));

        public static readonly DependencyProperty LineDataProperty =
            DependencyProperty.Register("LineData", typeof (ILineData), typeof (LineDataControl), new PropertyMetadata(null, ChangedLineData));

        public static readonly DependencyProperty NetworkFileNameProperty =
            DependencyProperty.Register("NetworkFileName", typeof (string), typeof (LineDataControl), new PropertyMetadata(null, ChangedNetworkFileName));

        public static readonly DependencyProperty SectionsToAddCountProperty =
            DependencyProperty.Register("SectionsToAddCount", typeof (int), typeof (LineDataControl), new PropertyMetadata(1));

        public static readonly DependencyProperty SelectedSectionIdProperty =
            DependencyProperty.Register("SelectedSectionId", typeof (string), typeof (LineDataControl), new PropertyMetadata(null, ChangedSelectedSectionId));

        public static readonly DependencyProperty SelectedVertexProperty =
            DependencyProperty.Register("SelectedVertex", typeof (Vertex), typeof (LineDataControl), new PropertyMetadata(null, ChangedLineData));

        public static readonly DependencyProperty SelectedYearProperty =
            DependencyProperty.Register("SelectedYear", typeof (int), typeof (LineDataControl), new PropertyMetadata(int.MinValue, ChangedSelectedYear));

        private readonly Dictionary<GridViewCellClipboardEventArgs, object> oldData = new Dictionary<GridViewCellClipboardEventArgs, object>();
        private readonly List<GridViewCellClipboardEventArgs> pastedCells = new List<GridViewCellClipboardEventArgs>();
        private readonly List<Tuple<int, int>> selectionInfos = new List<Tuple<int, int>>();
        private bool canUserInsertRows;
        private Network network;
        private ObservableCollection<Dynamic> rows;

        public bool CanUserInsertRows
        {
            get { return this.canUserInsertRows; }

            set
            {
                if (value.Equals(this.canUserInsertRows))
                {
                    return;
                }

                this.canUserInsertRows = value;
                this.RaisePropertyChanged();
            }
        }

        public string FileName
        {
            get { return (string) GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        public bool IsGridViewEnabled
        {
            get { return (bool) GetValue(IsGridViewEnabledProperty); }
            set { SetValue(IsGridViewEnabledProperty, value); }
        }

        public ILineData LineData
        {
            get { return (ILineData) GetValue(LineDataProperty); }

            set { SetValue(LineDataProperty, value); }
        }

        public string NetworkFileName
        {
            get { return (string) GetValue(NetworkFileNameProperty); }
            set { SetValue(NetworkFileNameProperty, value); }
        }

        public ObservableCollection<Dynamic> Rows
        {
            get { return this.rows; }

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
            get { return (int) GetValue(SectionsToAddCountProperty); }
            set { SetValue(SectionsToAddCountProperty, value); }
        }

        public string SelectedSectionId
        {
            get { return (string) GetValue(SelectedSectionIdProperty); }

            set { SetValue(SelectedSectionIdProperty, value); }
        }

        public Vertex SelectedVertex
        {
            get { return (Vertex) GetValue(SelectedVertexProperty); }
            set { SetValue(SelectedVertexProperty, value); }
        }

        public int SelectedYear
        {
            get { return (int) GetValue(SelectedYearProperty); }
            set { SetValue(SelectedYearProperty, value); }
        }

        public LineDataControl()
        {
            InitializeComponent();

            this.Loaded += LineDataControl_Loaded;
        }

        private static void ChangedLineData(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LineDataControl;

            if (!control.IsVisible)
            {
                return;
            }

            control.IsGridViewEnabled = true;

            if (control.LineData == null || control.SelectedVertex == null)
            {
                return;
            }

            var lineData = control.LineData;
            var vertexKey = control.SelectedVertex.Key;

            control.Rows = new ObservableCollection<Dynamic>();

            control.UpdateRows(lineData, vertexKey, new Progress<Dynamic>(row => control.Rows.Add(row)));

            foreach (var selectionInfo in control.selectionInfos)
            {
                try
                {
                    control.GridView.SelectedCells.Add(new GridViewCellInfo(control.Rows[selectionInfo.Item1], control.GridView.Columns[selectionInfo.Item2], control.GridView));
                }
                catch
                {
                    // do nothing
                }
            }

            control.LineData.DataChanged += control.LineData_DataChanged;
        }

        private static void ChangedNetworkFileName(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LineDataControl;

            if (e.OldValue == null || !e.OldValue.Equals(e.NewValue))
            {
                control.network = Network.Read(control.NetworkFileName);
            }
        }

        private static void ChangedSelectedSectionId(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LineDataControl;
            control.RunSection(control.SelectedSectionId);
        }

        private static void ChangedSelectedYear(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as LineDataControl).RaiseSelectedYearChanged();
        }

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null && propertyName != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void SetSelectedCells(VertexEvidence vertexEvidence)
        {
            foreach (var cell in this.GridView.SelectedCells)
            {
                var cellModel = cell.ToModel();

                if (!cellModel.IsColumnSectionId)
                {
                    this.SetCell(cellModel, vertexEvidence);
                }
            }
        }

        protected void RaiseEvidenceChanged(CellModel cellModel, VertexEvidence vertexEvidence)
        {
            if (this.EvidenceChanged != null)
            {
                this.EvidenceChanged(this, cellModel, vertexEvidence);
            }
        }

        protected void RaiseNotificationClosed(Notification notification)
        {
            if (this.NotificationClosed != null)
            {
                this.NotificationClosed(this, notification);
            }
        }

        protected void RaiseNotificationOpened(Notification notification)
        {
            if (this.NotificationOpened != null)
            {
                this.NotificationOpened(this, notification);
            }
        }

        protected void RaiseSectionBeliefsChanged()
        {
            if (this.SectionBeliefsChanged != null)
            {
                this.SectionBeliefsChanged(this, new EventArgs());
            }
        }

        protected void RaiseSectionEvidencesChanged()
        {
            if (this.SectionEvidencesChanged != null)
            {
                this.SectionEvidencesChanged(this, new EventArgs());
            }
        }

        protected void RaiseSelectedCellChanged()
        {
            if (this.SelectedCellChanged != null)
            {
                this.SelectedCellChanged(this, new EventArgs());
            }
        }

        protected void RaiseSelectedYearChanged()
        {
            if (this.SelectedYearChanged != null)
            {
                this.SelectedYearChanged(this, new EventArgs());
            }
        }

        private void AddRow(string sectionId)
        {
            var row = new Dynamic();
            row[CellModel.SectionIdHeader] = sectionId;

            var sectionEvidence = this.LineData.GetSectionEvidence(sectionId);

            for (var year = this.LineData.StartYear; year <= this.LineData.EndYear; year++)
            {
                row[year.ToString()] = sectionEvidence[year][this.SelectedVertex.Key];
            }

            this.Rows.Add(row);
        }

        private void AddSectionsButton_Click(object sender, RoutedEventArgs e)
        {
            var nSection = 1;

            for (var i = 0; i < this.SectionsToAddCount; i++)
            {
                var sectionId = "Section " + nSection;

                while (this.LineData.ContainsSection(sectionId))
                {
                    nSection++;
                    sectionId = "Section " + nSection;
                }

                this.LineData.AddSection(sectionId);
                this.AddRow(sectionId);
            }
        }

        private void CopyAcrossAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.GridView.SelectedCells.Count < 1)
            {
                return;
            }

            var selectedCellModel = this.GridView.SelectedCells[0].ToModel();

            if (selectedCellModel.IsColumnSectionId)
            {
                return;
            }

            foreach (var row in this.Rows)
            {
                foreach (var column in this.GridView.Columns)
                {
                    var cellModel = new CellModel(row, column.Header as string);

                    if (cellModel.IsColumnSectionId)
                    {
                        continue;
                    }

                    this.SetCell(cellModel, selectedCellModel.Data as VertexEvidence);
                }
            }
        }

        private void CopyAcrossColButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.GridView.SelectedCells.Count < 1)
            {
                return;
            }

            var isRow = false;
            var sectionIds = new List<string>();

            foreach (var selectedCell in this.GridView.SelectedCells)
            {
                sectionIds.AddUnique(selectedCell.ToModel().SectionId);
            }

            isRow = sectionIds.Count == 1;

            if (this.GridView.SelectedCells[0].ToModel().IsColumnSectionId || !isRow)
            {
                return;
            }

            foreach (var selectedCell in this.GridView.SelectedCells)
            {
                var selectedCellModel = selectedCell.ToModel();

                foreach (var row in this.Rows)
                {
                    var cellModel = new CellModel(row, selectedCellModel.Header);
                    this.SetCell(cellModel, (selectedCellModel.Data as VertexEvidence));
                }
            }
        }

        private void CopyAcrossRowButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.GridView.SelectedCells.Count < 1)
            {
                return;
            }

            var isColumn = false;
            var indices = new List<int>();

            foreach (var selectedCell in this.GridView.SelectedCells)
            {
                indices.AddUnique(selectedCell.Column.DisplayIndex);
            }

            isColumn = indices.Count == 1;

            if (this.GridView.SelectedCells[0].ToModel().IsColumnSectionId || !isColumn)
            {
                return;
            }

            foreach (var selectedCell in this.GridView.SelectedCells)
            {
                var selectedCellModel = selectedCell.ToModel();

                foreach (var column in this.GridView.Columns)
                {
                    var cellModel = new CellModel(selectedCellModel.Row, column.Header as string);

                    if (cellModel.IsColumnSectionId)
                    {
                        continue;
                    }

                    this.SetCell(cellModel, (selectedCellModel.Data as VertexEvidence));
                }
            }
        }

        private void GridView_AutoGeneratingColumn(object sender, GridViewAutoGeneratingColumnEventArgs e)
        {
            e.Column.CellTemplateSelector = (CellTemplateSelector) this.FindResource("CellTemplateSelector");
            e.Column.MaxWidth = 100;
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

            var vertexEvidence = this.SelectedVertex.States.ParseEvidenceString(e.NewValue as string);

            if (vertexEvidence.Type == VertexEvidenceType.Invalid)
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
            this.SelectedSectionId = cellModel.SectionId;

            if (cellModel.IsColumnSectionId)
            {
                this.GridView.SelectionUnit = GridViewSelectionUnit.FullRow;
                return;
            }

            this.SelectedYear = cellModel.Year;
            this.GridView.SelectionUnit = GridViewSelectionUnit.Cell;

            this.RaiseSelectedCellChanged();
        }

        private void GridView_Deleted(object sender, GridViewDeletedEventArgs e)
        {
            foreach (var item in e.Items)
            {
                var row = item as Dynamic;
                var sectionId = row[CellModel.SectionIdHeader] as string;
                this.LineData.RemoveSection(sectionId);
            }
        }

        private void GridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                foreach (var selectedCell in this.GridView.SelectedCells)
                {
                    var cellModel = selectedCell.ToModel();

                    if (cellModel.IsColumnSectionId)
                    {
                        this.Rows.Remove(row => row[CellModel.SectionIdHeader].Equals(cellModel.SectionId));
                        this.LineData.RemoveSection(cellModel.SectionId);
                    }
                    else
                    {
                        var selectedRow = this.Rows.First(row => row[CellModel.SectionIdHeader].Equals(cellModel.SectionId));
                        var selectedRowIndex = this.Rows.IndexOf(selectedRow);

                        this.Rows.Remove(selectedRow);
                        selectedRow[cellModel.Year.ToString()] = new VertexEvidence();
                        this.Rows.Insert(selectedRowIndex, selectedRow);
                    }
                }
            }
        }

        private void GridView_Pasted(object sender, RadRoutedEventArgs e)
        {
            foreach (var pastedCell in this.pastedCells)
            {
                var cellModel = pastedCell.Cell.ToModel();
                this.SetCell(cellModel, pastedCell.Value as string, this.oldData[pastedCell] as string);
            }

            this.CanUserInsertRows = false;
            this.pastedCells.Clear();
            this.RaiseSectionEvidencesChanged();
        }

        private void GridView_PastingCellClipboardContent(object sender, GridViewCellClipboardEventArgs e)
        {
            var cellModel = e.Cell.ToModel();

            this.CanUserInsertRows = cellModel.IsColumnSectionId;

            this.pastedCells.Add(e);
            this.oldData[e] = cellModel.Data;
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

            this.GridView.Deleted -= GridView_Deleted;
            this.GridView.Deleted += GridView_Deleted;

            this.GridView.KeyDown -= GridView_KeyDown;
            this.GridView.KeyDown += GridView_KeyDown;

            this.GridView.Pasted -= GridView_Pasted;
            this.GridView.Pasted += GridView_Pasted;

            this.GridView.PastingCellClipboardContent -= GridView_PastingCellClipboardContent;
            this.GridView.PastingCellClipboardContent += GridView_PastingCellClipboardContent;

            this.AddSectionsButton.Click -= AddSectionsButton_Click;
            this.AddSectionsButton.Click += AddSectionsButton_Click;

            this.CopyAcrossAllButton.Click -= CopyAcrossAllButton_Click;
            this.CopyAcrossAllButton.Click += CopyAcrossAllButton_Click;

            this.CopyAcrossColButton.Click -= CopyAcrossColButton_Click;
            this.CopyAcrossColButton.Click += CopyAcrossColButton_Click;

            this.CopyAcrossRowButton.Click -= CopyAcrossRowButton_Click;
            this.CopyAcrossRowButton.Click += CopyAcrossRowButton_Click;

            this.OpenButton.Click -= OpenButton_Click;
            this.OpenButton.Click += OpenButton_Click;

            this.RunAllButton.Click -= RunAllButton_Click;
            this.RunAllButton.Click += RunAllButton_Click;

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
                Filter = Marv.LineData.FileDescription + "|*." + Marv.LineData.FileExtension,
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                this.FileName = dialog.FileName;
                this.LineData = Utils.ReadJson<LineData>(this.FileName);
            }
        }

        private async void RunAllButton_Click(object sender, RoutedEventArgs e)
        {
            var lineData = this.LineData;

            var notification = new Notification
            {
                Description = "Running Model"
            };

            this.RaiseNotificationOpened(notification);

            await Task.Run(() => this.RunAllSections(lineData, new Progress<double>(progress => notification.Value = progress * 100)));

            this.RaiseNotificationClosed(notification);

            this.RaiseSectionBeliefsChanged();
        }

        private void RunAllSections(ILineData lineData, IProgress<double> progress)
        {
            var sectionIds = lineData.GetSectionIds().ToList();
            var total = sectionIds.Count;
            var done = 0.0;

            foreach (var sectionId in sectionIds)
            {
                var sectionEvidence = lineData.GetSectionEvidence(sectionId);
                lineData.SetSectionBelief(sectionId, this.network.Run(sectionEvidence));

                done++;
                progress.Report(done / total);
            }
        }

        private void RunSection(string sectionId)
        {
            var sectionEvidence = this.LineData.GetSectionEvidence(sectionId);

            var sectionBelief = this.network.Run(sectionEvidence);

            this.LineData.SetSectionBelief(sectionId, sectionBelief);

            this.RaiseSectionBeliefsChanged();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.FileName == null)
            {
                var dialog = new SaveFileDialog
                {
                    Filter = Marv.LineData.FileDescription + "|*." + Marv.LineData.FileExtension,
                };

                var result = dialog.ShowDialog();

                if (result == true)
                {
                    this.FileName = dialog.FileName;
                }
            }

            if (this.FileName != null)
            {
                if (Path.GetExtension(this.FileName) != "." + Marv.LineData.FileExtension)
                {
                    this.FileName = this.FileName + "." + Marv.LineData.FileExtension;
                }

                this.LineData.WriteJson(this.FileName);
            }
        }

        private void SetCell(CellModel cellModel, VertexEvidence vertexEvidence)
        {
            if (cellModel.IsColumnSectionId)
            {
                return;
            }

            cellModel.Data = vertexEvidence;
            this.LineData.GetSectionEvidence(cellModel.SectionId)[cellModel.Year][this.SelectedVertex.Key] = vertexEvidence;
        }

        private void SetCell(CellModel cellModel, string newString, string oldString = null)
        {
            if (cellModel.IsColumnSectionId)
            {
                if (oldString == null)
                {
                    this.LineData.SetSectionEvidence(newString, new Dict<int, string, VertexEvidence>());
                }
                else
                {
                    this.LineData.ReplaceSectionId(oldString, newString);
                }

                cellModel.Data = newString;
            }
            else
            {
                var vertexEvidence = this.SelectedVertex.States.ParseEvidenceString(newString);

                cellModel.Data = vertexEvidence;
                this.LineData.GetSectionEvidence(cellModel.SectionId)[cellModel.Year][this.SelectedVertex.Key] = vertexEvidence;

                this.RaiseEvidenceChanged(cellModel, vertexEvidence);
            }
        }

        private void UpdateRows()
        {
            this.UpdateRows(this.LineData, this.SelectedVertex.Key);
        }

        private void UpdateRows(ILineData lineData, string vertexKey, IProgress<Dynamic> progress = null)
        {
            foreach (var sectionId in lineData.GetSectionIds())
            {
                var row = new Dynamic();
                row[CellModel.SectionIdHeader] = sectionId;

                var sectionEvidence = lineData.GetSectionEvidence(sectionId);

                for (var year = lineData.StartYear; year <= lineData.EndYear; year++)
                {
                    row[year.ToString()] = sectionEvidence[year][vertexKey];
                }

                if (progress != null)
                {
                    progress.Report(row);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<CellModel, VertexEvidence> EvidenceChanged;

        public event EventHandler<Notification> NotificationOpened;

        public event EventHandler<Notification> NotificationClosed;

        public event EventHandler SelectedYearChanged;

        public event EventHandler SectionBeliefsChanged;

        public event EventHandler SectionEvidencesChanged;

        public event EventHandler SelectedCellChanged;
    }
}