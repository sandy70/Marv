﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Marv.Common;
using Marv.Common.Types;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Marv.Input
{
    public partial class LineDataControl : INotifyPropertyChanged
    {
        public static readonly DependencyProperty EndYearProperty =
            DependencyProperty.Register("EndYear", typeof (int), typeof (LineDataControl), new PropertyMetadata(2010));

        public static readonly DependencyProperty NetworkFileNameProperty =
            DependencyProperty.Register("NetworkFileName", typeof (string), typeof (LineDataControl), new PropertyMetadata(null));

        public static readonly DependencyProperty SectionsToAddCountProperty =
            DependencyProperty.Register("SectionsToAddCount", typeof (int), typeof (LineDataControl), new PropertyMetadata(1));

        public static readonly DependencyProperty SelectedSectionIdProperty =
            DependencyProperty.Register("SelectedSectionId", typeof (string), typeof (LineDataControl), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedYearProperty =
            DependencyProperty.Register("SelectedYear", typeof (int), typeof (LineDataControl), new PropertyMetadata(int.MinValue));

        public static readonly DependencyProperty StartYearProperty =
            DependencyProperty.Register("StartYear", typeof (int), typeof (LineDataControl), new PropertyMetadata(2010));

        private readonly Dictionary<GridViewCellClipboardEventArgs, object> oldData = new Dictionary<GridViewCellClipboardEventArgs, object>();
        private readonly List<GridViewCellClipboardEventArgs> pastedCells = new List<GridViewCellClipboardEventArgs>();
        private bool canUserInsertRows;
        private ObservableCollection<Dynamic> rows;
        private GridViewSelectionUnit selectionUnit = GridViewSelectionUnit.Cell;

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

        public int EndYear
        {
            get { return (int) GetValue(EndYearProperty); }
            set { SetValue(EndYearProperty, value); }
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

        public int SelectedYear
        {
            get { return (int) GetValue(SelectedYearProperty); }
            set { SetValue(SelectedYearProperty, value); }
        }

        public GridViewSelectionUnit SelectionUnit
        {
            get { return this.selectionUnit; }

            set
            {
                if (value.Equals(this.selectionUnit))
                {
                    return;
                }

                this.selectionUnit = value;
                this.RaisePropertyChanged();
            }
        }

        public int StartYear
        {
            get { return (int) GetValue(StartYearProperty); }
            set { SetValue(StartYearProperty, value); }
        }

        public LineDataControl()
        {
            InitializeComponent();
        }

        public void AddRow(string sectionId, Dict<int, VertexEvidence> vertexEvidences)
        {
            var row = new Dynamic();
            row[CellModel.SectionIdHeader] = sectionId;

            for (var year = this.StartYear; year <= this.EndYear; year++)
            {
                row[year.ToString()] = vertexEvidences[year];
            }

            this.Rows.Add(row);
        }

        public void ClearRows()
        {
            this.Rows = new ObservableCollection<Dynamic>();
        }

        public void ClearSelectedCell()
        {
            if (this.GridView.CurrentCellInfo == null)
            {
                return;
            }

            var cellModel = this.GridView.CurrentCellInfo.ToModel();

            if (!cellModel.IsColumnSectionId)
            {
                this.ClearCell(cellModel);
            }
        }

        public void SetCell(string sectionId, int year, VertexEvidence vertexEvidence)
        {
            var selectedRow = this.Rows.First(row => (new CellModel(row, year.ToString()).SectionId == sectionId));
            var cellModel = new CellModel(selectedRow, year.ToString());

            this.SetCell(cellModel, vertexEvidence);
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

        private void AddRow(string sectionId)
        {
            var row = new Dynamic();
            row[CellModel.SectionIdHeader] = sectionId;

            for (var year = this.StartYear; year <= this.EndYear; year++)
            {
                row[year.ToString()] = "";
            }

            this.Rows.Add(row);

            this.RaiseRowAdded(sectionId);
        }

        private void AddSectionsButton_Click(object sender, RoutedEventArgs e)
        {
            var nSection = 1;
            var sectionIds = this.Rows.Select(row => row[CellModel.SectionIdHeader] as string).ToList();

            for (var i = 0; i < this.SectionsToAddCount; i++)
            {
                var sectionId = "Section " + nSection;

                while (sectionIds.Contains(sectionId))
                {
                    nSection++;
                    sectionId = "Section " + nSection;
                }

                sectionIds.Add(sectionId);

                this.AddRow(sectionId);
            }
        }

        private void ClearCell(CellModel cellModel)
        {
            var selectedRow = this.Rows.First(row => row[CellModel.SectionIdHeader].Equals(cellModel.SectionId));
            var selectedRowIndex = this.Rows.IndexOf(selectedRow);

            this.Rows.Remove(selectedRow);
            selectedRow[cellModel.Year.ToString()] = new VertexEvidence();
            this.Rows.Insert(selectedRowIndex, selectedRow);

            this.RaiseCellContentChanged(new CellChangedEventArgs
            {
                CellModel = cellModel,
                NewString = ""
            });
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

            var sectionIds = new List<string>();

            foreach (var selectedCell in this.GridView.SelectedCells)
            {
                sectionIds.AddUnique(selectedCell.ToModel().SectionId);
            }

            var isRow = sectionIds.Count == 1;

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

            var indices = new List<int>();

            foreach (var selectedCell in this.GridView.SelectedCells)
            {
                indices.AddUnique(selectedCell.Column.DisplayIndex);
            }

            var isColumn = indices.Count == 1;

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

        private void EndYear_ValueChanged(object sender, RadRangeBaseValueChangedEventArgs e)
        {
            if (this.Rows == null)
            {
                return;
            }

            var oldStartYear = int.MinValue;

            if (this.EndYear < this.StartYear)
            {
                oldStartYear = this.StartYear;
                this.StartYear = this.EndYear;
            }

            if (e.NewValue != null && e.OldValue != null)
            {
                this.Rows = this.GetNewRows((int) e.NewValue, (int) e.OldValue, this.StartYear, oldStartYear);
            }
        }

        private ObservableCollection<Dynamic> GetNewRows(int newEndYear, int oldEndYear, int newStartYear, int oldStartYear)
        {
            var newRows = new ObservableCollection<Dynamic>();

            foreach (var oldRow in this.Rows)
            {
                var newRow = new Dynamic();

                newRow[CellModel.SectionIdHeader] = oldRow[CellModel.SectionIdHeader];

                for (var year = newStartYear; year <= newEndYear; year++)
                {
                    if (year < oldStartYear || oldEndYear < year)
                    {
                        // Data does not exist in the old row
                        newRow[year.ToString()] = new VertexEvidence();
                    }
                    else
                    {
                        newRow[year.ToString()] = oldRow[year.ToString()];
                    }
                }

                newRows.Add(newRow);
            }

            return newRows;
        }

        private void RaiseCellContentChanged(CellChangedEventArgs cellChangedEventArgs)
        {
            if (this.CellContentChanged != null)
            {
                this.CellContentChanged(this, cellChangedEventArgs);
            }
        }

        private void RaiseCellValidating(GridViewCellValidatingEventArgs e)
        {
            if (this.CellValidating != null)
            {
                this.CellValidating(this, e);
            }
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null && propertyName != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void RaiseRowAdded(string sectionId)
        {
            if (this.RowAdded != null)
            {
                this.RowAdded(this, sectionId);
            }
        }

        private void RaiseRowRemoved(string sectionId)
        {
            if (this.RowRemoved != null)
            {
                this.RowRemoved(this, sectionId);
            }
        }

        private void RaiseRowSelected(CellModel cellModel)
        {
            if (this.RowSelected != null)
            {
                this.RowSelected(this, cellModel);
            }
        }

        private void RaiseSectionIdPasting(GridViewCellClipboardEventArgs e)
        {
            if (this.SectionIdPasting != null)
            {
                this.SectionIdPasting(this, e);
            }
        }

        private void RaiseSectionIdValidating(GridViewCellValidatingEventArgs e)
        {
            if (this.SectionIdValidating != null)
            {
                this.SectionIdValidating(this, e);
            }
        }

        private void RaiseSelectedCellChanged()
        {
            if (this.SelectedCellChanged != null)
            {
                this.SelectedCellChanged(this, new EventArgs());
            }
        }

        private void SetCell(CellModel cellModel, VertexEvidence vertexEvidence)
        {
            if (cellModel.IsColumnSectionId)
            {
                return;
            }

            cellModel.Data = vertexEvidence;

            this.RaiseCellContentChanged(new CellChangedEventArgs
            {
                CellModel = cellModel,
                VertexEvidence = cellModel.Data as VertexEvidence
            });
        }

        private void StartYear_ValueChanged(object sender, RadRangeBaseValueChangedEventArgs e)
        {
            if (this.Rows == null)
            {
                return;
            }

            var oldEndYear = int.MaxValue;

            if (this.StartYear > this.EndYear)
            {
                oldEndYear = this.EndYear;
                this.EndYear = this.StartYear;
            }

            if (e.NewValue != null && e.OldValue != null)
            {
                this.Rows = this.GetNewRows(this.EndYear, oldEndYear, (int) e.NewValue, (int) e.OldValue);
            }
        }

        public event EventHandler<CellModel> RowSelected;

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler SelectedCellChanged;

        public event EventHandler<GridViewCellValidatingEventArgs> CellValidating;

        public event EventHandler<CellChangedEventArgs> CellContentChanged;

        public event EventHandler<string> RowAdded;

        public event EventHandler<string> RowRemoved;

        public event EventHandler<GridViewCellValidatingEventArgs> SectionIdValidating;

        public event EventHandler<GridViewCellClipboardEventArgs> SectionIdPasting;
    }
}