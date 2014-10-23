using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Marv.Common;
using Marv.Common.Graph;
using Microsoft.Win32;
using Telerik.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Marv.Input
{
    public partial class LineDataControl : INotifier, INotifyPropertyChanged
    {
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
        private Network network;
        private ObservableCollection<Dynamic> rows;

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

        private static void AddRow(ObservableCollection<Dynamic> rows, ILineData lineData, string vertexKey, string sectionId)
        {
            var sectionEvidence = lineData.GetSectionEvidence(sectionId);

            var row = new Dynamic();
            row[CellModel.SectionIdHeader] = sectionId;

            for (var year = lineData.StartYear; year <= lineData.EndYear; year++)
            {
                row[year.ToString()] = sectionEvidence[year][vertexKey];
            }

            rows.Add(row);
        }

        private static void AddSections(ObservableCollection<Dynamic> rows, ILineData lineData, string vertexKey, int sectionsToAddCount, IProgress<double> progress = null)
        {
            var count = 0.0;
            var nSection = 1;

            for (var i = 0; i < sectionsToAddCount; i++)
            {
                var sectionId = "Section " + nSection;

                while (lineData.ContainsSection(sectionId))
                {
                    sectionId = "Section " + nSection++;
                }

                lineData.SetSectionEvidence(sectionId, new Dict<int, string, VertexEvidence>());

                AddRow(rows, lineData, vertexKey, sectionId);

                if (progress != null)
                {
                    progress.Report(count++ / sectionsToAddCount);
                }

                Thread.Sleep(1);
            }
        }

        private static async void ChangedLineData(DependencyObject d, DependencyPropertyChangedEventArgs e)
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

            var rows = control.Rows = new ObservableCollection<Dynamic>();

            var notification = new Notification
            {
                Description = "Reading line data...",
                Value = 0
            };

            control.RaiseNotificationOpened(notification);

            await Task.Run(() => UpdateRows(rows, lineData, vertexKey, new Progress<double>(p => notification.Value = p)));

            control.RaiseNotificationClosed(notification);

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

        private static void UpdateRows(ObservableCollection<Dynamic> rows, ILineData lineData, string vertexKey, IProgress<double> progress)
        {
            var count = 1.0;
            var total = lineData.GetSectionIds().Count();

            foreach (var sectionId in lineData.GetSectionIds())
            {
                AddRow(rows, lineData, vertexKey, sectionId);

                if (progress != null)
                {
                    progress.Report(count ++ / total);
                }

                Thread.Sleep(1);
            }
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

        private async void AddSectionsButton_Click(object sender, RoutedEventArgs e)
        {
            var lineData = this.LineData;
            var theRows = this.Rows;
            var sectionsToAddCount = this.SectionsToAddCount;
            var vertexKey = this.SelectedVertex.Key;

            var notification = new Notification
            {
                IsIndeterminate = true,
                Description = "Adding sections..."
            };

            this.RaiseNotificationOpened(notification);

            this.IsEnabled = false;

            await Task.Run(() => AddSections(theRows, lineData, vertexKey, sectionsToAddCount));

            this.IsEnabled = true;

            this.RaiseNotificationClosed(notification);
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

            var vertexData = this.SelectedVertex.States.ParseEvidenceString(e.NewValue as string);

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
                this.LineData.RemoveSection(sectionId);
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
                Filter = Common.Graph.LineData.FileDescription + "|*." + Common.Graph.LineData.FileExtension,
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                this.FileName = dialog.FileName;

                var directoryName = Path.GetDirectoryName(this.FileName);

                if (Directory.Exists(Path.Combine(directoryName, "SectionBeliefs")) && Directory.Exists(Path.Combine(directoryName, "SectionEvidences")))
                {
                    // This is a folder line data
                    this.LineData = FolderLineData.Read(this.FileName);
                }
                else
                {
                    this.LineData = Common.Graph.LineData.Read(this.FileName);
                }
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

            // this.LineData.SectionBeliefs = await Task.Run(() => this.network.Run(sectionEvidences, new Progress<double>(progress => notification.Value = progress * 100)));

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
                    Filter = Common.Graph.LineData.FileDescription + "|*." + Common.Graph.LineData.FileExtension,
                };

                var result = dialog.ShowDialog();

                if (result == true)
                {
                    this.FileName = dialog.FileName;
                }
            }

            if (this.FileName != null)
            {
                if (Path.GetExtension(this.FileName) != "." + Common.Graph.LineData.FileExtension)
                {
                    this.FileName = this.FileName + "." + Common.Graph.LineData.FileExtension;
                }

                this.LineData.Write(this.FileName);
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
                    this.LineData.ChangeSectionId(oldString, newString);
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

                Thread.Sleep(1);
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