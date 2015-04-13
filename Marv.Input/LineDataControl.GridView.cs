using System;
using System.Linq;
using System.Windows;
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
            this.RaiseCellContentChanged(new CellChangedEventArgs
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
                this.RaiseSectionIdValidating(e);
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
                this.CanUserInsertRows = true;
                this.SelectionUnit = GridViewSelectionUnit.FullRow;
                this.RaiseRowSelected(cellModel.SectionId);
                return;
            }

            this.CanUserInsertRows = false;
            this.SelectedYear = cellModel.Year;
            this.SelectionUnit = GridViewSelectionUnit.Cell;

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
                this.RaiseCellContentChanged(new CellChangedEventArgs
                {
                    CellModel = pastedCell.Cell.ToModel(),
                    NewString = pastedCell.Value as string,
                    OldString = this.oldData[pastedCell] as string
                });
            }

            this.CanUserInsertRows = false;
            this.pastedCells.Clear();
        }

        private void GridView_Pasting(object sender, GridViewClipboardEventArgs e)
        {
            var text = Clipboard.GetText(TextDataFormat.CommaSeparatedValue);

            var lines = text.Trim().Split(new[] { "\r\n" }, StringSplitOptions.None);
            var values = new string[lines.Count()][];

            var row = 0;
            foreach (var line in lines)
            {
                var parts = line.Split(",".ToCharArray());

                values[row] = new string[parts.Count()];

                var col = 0;
                foreach (var part in parts)
                {
                    values[row][col] = part;
                    col++;
                }

                row++;
            }

            Console.WriteLine(values);
        }

        private void GridView_PastingCellClipboardContent(object sender, GridViewCellClipboardEventArgs e)
        {
            var cellModel = e.Cell.ToModel();

            if (cellModel.IsColumnSectionId)
            {
                this.RaiseSectionIdPasting(e);
            }

            if (!e.Cancel)
            {
                this.pastedCells.Add(e);
                this.oldData[e] = cellModel.Data;
            }
        }
    }
}