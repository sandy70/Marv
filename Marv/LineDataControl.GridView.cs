using System.Windows;
using System.Windows.Input;
using Marv.Input;
using Telerik.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Marv
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
                if (this.GridView.SelectionUnit == GridViewSelectionUnit.Cell)
                {
                    foreach (var selectedCell in this.GridView.SelectedCells)
                    {
                        var cellModel = selectedCell.ToModel();
                        this.SetCell(cellModel, "");
                    }
                }
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

        private void LineDataControl_Loaded_GridView(object sender, RoutedEventArgs e)
        {
            this.GridView.AutoGeneratingColumn -= this.GridView_AutoGeneratingColumn;
            this.GridView.AutoGeneratingColumn += this.GridView_AutoGeneratingColumn;

            this.GridView.CellEditEnded -= this.GridView_CellEditEnded;
            this.GridView.CellEditEnded += this.GridView_CellEditEnded;

            this.GridView.CellValidating -= this.GridView_CellValidating;
            this.GridView.CellValidating += this.GridView_CellValidating;

            this.GridView.CurrentCellChanged -= this.GridView_CurrentCellChanged;
            this.GridView.CurrentCellChanged += this.GridView_CurrentCellChanged;

            this.GridView.Deleted -= this.GridView_Deleted;
            this.GridView.Deleted += this.GridView_Deleted;

            this.GridView.KeyDown -= this.GridView_KeyDown;
            this.GridView.KeyDown += this.GridView_KeyDown;

            this.GridView.Pasted -= this.GridView_Pasted;
            this.GridView.Pasted += this.GridView_Pasted;

            this.GridView.PastingCellClipboardContent -= this.GridView_PastingCellClipboardContent;
            this.GridView.PastingCellClipboardContent += this.GridView_PastingCellClipboardContent;
        }
    }
}