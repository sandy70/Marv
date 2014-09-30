using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
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
            DependencyProperty.Register("LineData", typeof (LineData), typeof (LineDataControl), new PropertyMetadata(null, ChangedLineData));

        public static readonly DependencyProperty NetworkFileNameProperty =
            DependencyProperty.Register("NetworkFileName", typeof (string), typeof (LineDataControl), new PropertyMetadata(null, ChangedNetworkFileName));

        public static readonly DependencyProperty SectionsToAddCountProperty =
            DependencyProperty.Register("SectionsToAddCount", typeof (int), typeof (LineDataControl), new PropertyMetadata(1));

        public static readonly DependencyProperty SelectedSectionIdProperty =
            DependencyProperty.Register("SelectedSectionId", typeof (string), typeof (LineDataControl), new PropertyMetadata(null, ChangedSelectedSectionId));

        public static readonly DependencyProperty SelectedVertexProperty =
            DependencyProperty.Register("SelectedVertex", typeof (Vertex), typeof (LineDataControl), new PropertyMetadata(null, ChangedVertex));

        public static readonly DependencyProperty SelectedYearProperty =
            DependencyProperty.Register("SelectedYear", typeof (int), typeof (LineDataControl), new PropertyMetadata(int.MinValue, ChangedSelectedYear));

        private readonly Dictionary<GridViewCellClipboardEventArgs, object> oldData = new Dictionary<GridViewCellClipboardEventArgs, object>();
        private readonly List<GridViewCellClipboardEventArgs> pastedCells = new List<GridViewCellClipboardEventArgs>();
        private readonly List<Tuple<int, int>> selectionInfos = new List<Tuple<int, int>>();
        private Network network;
        private ObservableCollection<Dynamic> rows;

        public Dict<string, VertexEvidence> CurrentGraphData
        {
            get
            {
                return (Dict<string, VertexEvidence>) GetValue(CurrentGraphDataProperty);
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

        public bool IsGridViewEnabled
        {
            get
            {
                return (bool) GetValue(IsGridViewEnabledProperty);
            }
            set
            {
                SetValue(IsGridViewEnabledProperty, value);
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

        public string NetworkFileName
        {
            get
            {
                return (string) GetValue(NetworkFileNameProperty);
            }
            set
            {
                SetValue(NetworkFileNameProperty, value);
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

        public string SelectedSectionId
        {
            get
            {
                return (string) GetValue(SelectedSectionIdProperty);
            }

            set
            {
                SetValue(SelectedSectionIdProperty, value);
            }
        }

        public Vertex SelectedVertex
        {
            get
            {
                return (Vertex) GetValue(SelectedVertexProperty);
            }
            set
            {
                SetValue(SelectedVertexProperty, value);
            }
        }

        public int SelectedYear
        {
            get
            {
                return (int) GetValue(SelectedYearProperty);
            }
            set
            {
                SetValue(SelectedYearProperty, value);
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

            control.IsGridViewEnabled = true;

            control.UpdateRows();

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

        private static void ChangedVertex(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LineDataControl;
            control.UpdateRows();
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

        private void AddSectionsButton_Click(object sender, RoutedEventArgs e)
        {
            var nSection = 1;

            var keys = Enumerable.Range(0, this.SectionsToAddCount).Select(i =>
            {
                var sectionId = "Section " + nSection;

                while (this.LineData.SectionEvidences.ContainsKey(sectionId))
                {
                    nSection++;
                    sectionId = "Section " + nSection;
                }

                return sectionId;
            });

            this.LineData.AddSections(keys);
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

            var selectedCellModel = this.GridView.SelectedCells[0].ToModel();

            if (selectedCellModel.IsColumnSectionId)
            {
                return;
            }

            foreach (var row in this.Rows)
            {
                var cellModel = new CellModel(row, selectedCellModel.Header);
                this.SetCell(cellModel, (selectedCellModel.Data as VertexEvidence));
            }
        }

        private void CopyAcrossRowButton_Click(object sender, RoutedEventArgs e)
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

            var vertexData = this.SelectedVertex.ParseEvidenceString(e.NewValue as string);

            if (vertexData.Type == VertexEvidenceType.Invalid)
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
                this.LineData.SectionEvidences[sectionId] = null;
            }
        }

        private void GridView_Pasted(object sender, RadRoutedEventArgs e)
        {
            foreach (var pastedCell in this.pastedCells)
            {
                this.SetCell(pastedCell.Cell.ToModel(), pastedCell.Value as string, this.oldData[pastedCell] as string);
            }

            this.pastedCells.Clear();

            this.RaiseSectionEvidencesChanged();
        }

        private void GridView_PastingCellClipboardContent(object sender, GridViewCellClipboardEventArgs e)
        {
            this.pastedCells.Add(e);
            this.oldData[e] = e.Cell.ToModel().Data;
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
                Filter = LineData.FileDescription + "|*." + LineData.FileExtension,
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
            var lineData = this.LineData.SectionEvidences;

            var notification = new Notification
            {
                Description = "Running Model"
            };

            this.RaiseNotificationOpened(notification);

            this.LineData.SectionBeliefs = await Task.Run(() => this.network.Run(lineData, new Progress<double>(progress => notification.Value = progress * 100)));

            this.RaiseNotificationClosed(notification);

            this.RaiseSectionBeliefsChanged();
        }

        private void RunSection(string sectionId)
        {
            this.LineData.SectionBeliefs[sectionId] = this.network.Run(this.LineData.SectionEvidences[sectionId]);
            this.RaiseSectionBeliefsChanged();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.FileName == null)
            {
                var dialog = new SaveFileDialog
                {
                    Filter = LineData.FileDescription + "|" + LineData.FileExtension,
                };

                var result = dialog.ShowDialog();

                if (result == true)
                {
                    this.FileName = dialog.FileName;
                }
            }

            if (this.FileName != null)
            {
                if (Path.GetExtension(this.FileName) != "." + LineData.FileExtension)
                {
                    this.FileName = this.FileName + "." + LineData.FileExtension;
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
            this.LineData.SectionEvidences[cellModel.SectionId][cellModel.Year][this.SelectedVertex.Key] = vertexEvidence;

            this.RunSection(cellModel.SectionId);
        }

        private void SetCell(CellModel cellModel, string newString, string oldString = null)
        {
            if (cellModel.IsColumnSectionId)
            {
                if (oldString == null)
                {
                    this.LineData.SectionEvidences[newString] = new Dict<int, string, VertexEvidence>();
                }
                else
                {
                    this.LineData.SectionEvidences.ChangeKey(oldString, newString);
                }

                cellModel.Data = newString;
            }
            else
            {
                var vertexData = this.SelectedVertex.ParseEvidenceString(newString);

                cellModel.Data = vertexData;
                this.LineData.SectionEvidences[cellModel.SectionId][cellModel.Year][this.SelectedVertex.Key] = vertexData;

                this.RunSection(cellModel.SectionId);
            }
        }

        private void UpdateRows()
        {
            foreach (var selectedCell in this.GridView.SelectedCells)
            {
                this.selectionInfos.Add(new Tuple<int, int>(this.Rows.IndexOf(selectedCell.Item as Dynamic), selectedCell.Column.DisplayIndex));
            }

            if (this.LineData == null || this.SelectedVertex == null)
            {
                return;
            }

            var newRows = new ObservableCollection<Dynamic>();

            foreach (var sectionId in this.LineData.SectionEvidences.Keys)
            {
                var row = new Dynamic();
                row[CellModel.SectionIdHeader] = sectionId;

                for (var year = this.LineData.StartYear; year <= this.LineData.EndYear; year++)
                {
                    row[year.ToString()] = this.LineData.SectionEvidences[sectionId][year][this.SelectedVertex.Key];
                }

                newRows.Add(row);
            }

            this.Rows = newRows;

            foreach (var selectionInfo in this.selectionInfos)
            {
                try
                {
                    this.GridView.SelectedCells.Add(new GridViewCellInfo(this.Rows[selectionInfo.Item1], this.GridView.Columns[selectionInfo.Item2], this.GridView));
                }
                catch
                {
                    // do nothing
                }
            }
        }

        public event EventHandler<Notification> NotificationOpened;

        public event EventHandler<Notification> NotificationClosed;

        public event EventHandler SelectedYearChanged;

        public event EventHandler SectionBeliefsChanged;

        public event EventHandler SectionEvidencesChanged;

        public event EventHandler SelectedCellChanged;
    }
}