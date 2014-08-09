using System.Collections.Generic;
using System.Windows.Input;
using Marv.Common.Graph;
using Telerik.Windows;
using Telerik.Windows.Controls;

namespace Marv.Input
{
    public partial class MainWindow
    {
        private readonly List<GridViewCellClipboardEventArgs> cellClipboardEventArgs = new List<GridViewCellClipboardEventArgs>();
        private readonly Dictionary<GridViewCellClipboardEventArgs, object> oldValues = new Dictionary<GridViewCellClipboardEventArgs, object>();

        public void SetCell(CellModel cellModel, string vertexKey, VertexEvidence evidence)
        {
            if (cellModel == null || cellModel.IsColumnSectionId) return;

            cellModel.Data = evidence;

            this.LineEvidence
                .SectionEvidences[cellModel.SectionId]
                .YearEvidences[cellModel.Year]
                .GraphEvidence[vertexKey] = evidence;
        }

        public void SetCell(CellModel cellModel, string str)
        {
            if (cellModel.IsColumnSectionId)
            {
                this.LineEvidence.SectionEvidences.ReplaceKey(cellModel.SectionId, str);
                cellModel.Data = str;
                return;
            }

            var selectedVertex = this.Graph.SelectedVertex;

            if (selectedVertex == null) return;

            var values = this.Graph.SelectedVertex.States.Parse(str);

            this.SetCell(cellModel, selectedVertex.Key, new VertexEvidence(values, str));
        }

        private void InputGridView_AutoGeneratingColumn(object sender, GridViewAutoGeneratingColumnEventArgs e)
        {
            e.Column.CellTemplateSelector = this.InputGridView.FindResource("CellTemplateSelector") as CellTemplateSelector;
        }

        private void InputGridView_CellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {
            this.SetCell(e.Cell.ToModel(), e.NewData as string);
            this.Graph.Run();
        }

        private void InputGridView_CellValidating(object sender, GridViewCellValidatingEventArgs e)
        {
            if (e.Cell.ToModel().IsColumnSectionId) return;

            var evidenceString = e.NewValue as string;
            var vertexEvidence = this.Graph.SelectedVertex.States.Parse(evidenceString);

            if (vertexEvidence != null || evidenceString == string.Empty) return;

            e.IsValid = false;
            e.ErrorMessage = "Not a correct value or range of values. Press ESC to cancel.";
        }

        private void InputGridView_CurrentCellChanged(object sender, GridViewCurrentCellChangedEventArgs e)
        {
            if (e.NewCell == null) return;

            var cellModel = e.NewCell.ToModel();

            if (cellModel.IsColumnSectionId) return;

            var graphEvidence = this.LineEvidence.SectionEvidences[cellModel.SectionId].YearEvidences[cellModel.Year].GraphEvidence;

            this.Graph.SetEvidence(graphEvidence);
            this.Graph.Run();
        }

        private void InputGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Delete && e.Key != Key.Back)
            {
                return;
            }

            foreach (var cellInfo in this.InputGridView.SelectedCells)
            {
                this.SetCell(cellInfo.ToModel(), null);
            }
        }

        private void InputGridView_Pasted(object sender, RadRoutedEventArgs e)
        {
            foreach (var cellClipboardEventArg in this.cellClipboardEventArgs)
            {
                this.SetCell(cellClipboardEventArg.Cell.ToModel(), cellClipboardEventArg.Value as string);
            }

            cellClipboardEventArgs.Clear();
        }

        private void InputGridView_PastingCellClipboardContent(object sender, GridViewCellClipboardEventArgs e)
        {
            this.cellClipboardEventArgs.Add(e);
            this.oldValues[e] = e.Cell.ToModel().Data;
        }
    }
}