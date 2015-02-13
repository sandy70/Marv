using System.Windows.Input;
using Marv.Common;
using Telerik.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Marv.Input
{
    public partial class LineDataControl
    {
        private void GridView_AutoGeneratingColumn(object sender, GridViewAutoGeneratingColumnEventArgs e)
        {
            e.Column.CellTemplateSelector = (CellTemplateSelector) this.FindResource("CellTemplateSelector");
            e.Column.MaxWidth = 100;
        }

        private void GridView_CellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {
            this.RaiseCellChanged(new CellChangedEventArgs
            {
                CellModel = e.Cell.ToModel(),
                NewString = e.NewData as string,
                OldString = e.OldData as string
            });
        }

        private void GridView_CellValidating(object sender, GridViewCellValidatingEventArgs e)
        {
            if (e.Cell.ToModel().IsColumnSectionId)
            {
                return;
            }

            this.RaiseCellValidating(e);
        }

        private void GridView_CurrentCellChanged(object sender, GridViewCurrentCellChangedEventArgs e)
        {
            if (e.NewCell == null)
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
                
                this.RaiseRowRemoved(sectionId);
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
                        this.RaiseRowRemoved(cellModel.SectionId);
                    }
                    else
                    {
                        this.ClearCell(cellModel);
                    }
                }
            }
        }

        private void GridView_Pasted(object sender, RadRoutedEventArgs e)
        {
            foreach (var pastedCell in this.pastedCells)
            {
                this.RaiseCellChanged(new CellChangedEventArgs
                {
                    CellModel = pastedCell.Cell.ToModel(),
                    NewString = pastedCell.Value as string,
                    OldString = this.oldData[pastedCell] as string
                });
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
    }
}