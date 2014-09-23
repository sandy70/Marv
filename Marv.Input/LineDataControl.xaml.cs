﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
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
        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register("FileName", typeof (string), typeof (LineDataControl), new PropertyMetadata(null));

        public static readonly DependencyProperty GraphProperty =
            DependencyProperty.Register("Graph", typeof (Graph), typeof (LineDataControl), new PropertyMetadata(null));

        public static readonly DependencyProperty IsGridViewEnabledProperty =
            DependencyProperty.Register("IsGridViewEnabled", typeof (bool), typeof (LineDataControl), new PropertyMetadata(false));

        public static readonly DependencyProperty LineDataProperty =
            DependencyProperty.Register("LineData", typeof (LineData), typeof (LineDataControl), new PropertyMetadata(null, ChangedLineData));

        public static readonly DependencyProperty SectionsToAddCountProperty =
            DependencyProperty.Register("SectionsToAddCount", typeof (int), typeof (LineDataControl), new PropertyMetadata(1));

        public static readonly DependencyProperty SelectedSectionIdProperty =
            DependencyProperty.Register("SelectedSectionId", typeof (string), typeof (LineDataControl), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedVertexProperty =
            DependencyProperty.Register("SelectedVertex", typeof (Vertex), typeof (LineDataControl), new PropertyMetadata(null, ChangedVertex));

        public static readonly DependencyProperty SelectedYearProperty =
            DependencyProperty.Register("SelectedYear", typeof (int), typeof (LineDataControl), new PropertyMetadata(int.MinValue));

        private readonly Dictionary<GridViewCellClipboardEventArgs, object> oldData = new Dictionary<GridViewCellClipboardEventArgs, object>();
        private readonly List<GridViewCellClipboardEventArgs> pastedCells = new List<GridViewCellClipboardEventArgs>();
        private readonly List<Tuple<int, int>> selectionInfos = new List<Tuple<int, int>>();
        private ObservableCollection<Dynamic> rows;

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

        public Graph Graph
        {
            get
            {
                return (Graph) GetValue(GraphProperty);
            }
            set
            {
                SetValue(GraphProperty, value);
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

        public void SetSelectedCells(VertexData vertexData)
        {
            foreach (var cell in this.GridView.SelectedCells)
            {
                var cellModel = cell.ToModel();

                if (!cellModel.IsColumnSectionId)
                {
                    this.SetCell(cellModel, vertexData.String);
                }
            }
        }

        protected void RaiseNotificationIssued(Notification notification)
        {
            if (this.NotificationIssued != null)
            {
                this.NotificationIssued(this, notification);
            }
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

                    this.SetCell(cellModel, selectedCellModel.Data as VertexData);
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
                this.SetCell(cellModel, (selectedCellModel.Data as VertexData).String);
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

                this.SetCell(cellModel, (selectedCellModel.Data as VertexData).String);
            }
        }

        private void GridView_AutoGeneratingColumn(object sender, GridViewAutoGeneratingColumnEventArgs e)
        {
            e.Column.CellTemplateSelector = (CellTemplateSelector) this.FindResource("CellTemplateSelector");
            e.Column.MaxWidth = 100;
        }

        private async void GridView_CellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {
            var cellModel = e.Cell.ToModel();
            var oldString = e.OldData as string;
            var newString = e.NewData as string;

            this.SetCell(cellModel, newString, oldString);

            await this.RunSelectedSectionAsync();

            this.Graph.Data = this.LineData.Sections[this.SelectedSectionId][this.SelectedYear];
        }

        private void GridView_CellValidating(object sender, GridViewCellValidatingEventArgs e)
        {
            if (e.Cell.ToModel().IsColumnSectionId)
            {
                return;
            }

            var vertexEvidenceInfo = this.SelectedVertex.ParseEvidenceInfo(e.NewValue as string);

            if (vertexEvidenceInfo.Type == VertexEvidenceType.Invalid)
            {
                e.IsValid = false;
                e.ErrorMessage = "Not a correct value or range of values. Press ESC to cancel.";
            }
        }

        private async void GridView_CurrentCellChanged(object sender, GridViewCurrentCellChangedEventArgs e)
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

            await this.RunSelectedSectionAsync();

            this.SelectedYear = cellModel.Year;
            this.GridView.SelectionUnit = GridViewSelectionUnit.Cell;
            this.Graph.Data = this.LineData.Sections[cellModel.SectionId][cellModel.Year];
        }

        private void GridView_Deleted(object sender, GridViewDeletedEventArgs e)
        {
            foreach (var item in e.Items)
            {
                var row = item as Dynamic;
                var sectionId = row[CellModel.SectionIdHeader] as string;
                this.LineData.Sections[sectionId] = null;
            }
        }

        private void GridView_Pasted(object sender, RadRoutedEventArgs e)
        {
            foreach (var pastedCell in this.pastedCells)
            {
                this.SetCell(pastedCell.Cell.ToModel(), pastedCell.Value as string, this.oldData[pastedCell] as string);
            }

            this.pastedCells.Clear();
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
            var loops = this.Graph.Loops;
            var network = this.Graph.Network;
            var lineData = this.LineData.Sections;

            var notification = new Notification
            {
                Description = "Running Model"
            };

            this.RaiseNotificationIssued(notification);

            var progress = new Progress<double>(p => notification.Value = p * 100);

            await Task.Run(() => network.Run(lineData, loops, progress));

            this.Graph.Data = this.LineData.Sections[this.SelectedSectionId][this.SelectedYear];
        }

        private Task RunSelectedSectionAsync()
        {
            var loops = this.Graph.Loops;
            var network = this.Graph.Network;
            var sectionData = this.LineData.Sections[this.SelectedSectionId];

            return Task.Run(() => network.Run(sectionData, loops));
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
                if (Path.GetExtension(this.FileName) != LineData.FileExtension)
                {
                    this.FileName = this.FileName + "." + LineData.FileExtension;
                }

                this.LineData.WriteJson(this.FileName);
            }
        }

        private void SetCell(CellModel cellModel, VertexData vertexData)
        {
            if (cellModel.IsColumnSectionId)
            {
                return;
            }

            cellModel.Data = vertexData;
            this.LineData.Sections[cellModel.SectionId][cellModel.Year][this.SelectedVertex.Key] = vertexData;
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
                var distribution = this.SelectedVertex.States.Parse(newString);

                var vertexData = new VertexData();
                vertexData.Evidence = distribution == null ? null : distribution.ToArray();
                vertexData.String = newString;

                cellModel.Data = vertexData;
                this.LineData.Sections[cellModel.SectionId][cellModel.Year][this.SelectedVertex.Key] = vertexData;
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

            foreach (var sectionId in this.LineData.Sections.Keys)
            {
                var row = new Dynamic();
                row[CellModel.SectionIdHeader] = sectionId;

                for (var year = this.LineData.StartYear; year <= this.LineData.EndYear; year++)
                {
                    row[year.ToString()] = this.LineData.Sections[sectionId][year][this.SelectedVertex.Key];
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

        public event EventHandler<Notification> NotificationIssued;
    }
}