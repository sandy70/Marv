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

        public void SetCell(CellModel cellModel, Vertex vertex)
        {
            if (cellModel == null || cellModel.IsColumnSectionId) return;

            if (vertex == null) return;

            if (vertex.IsEvidenceEntered)
            {
                var vertexData = vertex.GetData();

                cellModel.Data = vertexData;

                this.LineEvidence
                    .SectionEvidences[cellModel.SectionId]
                    .YearEvidences[cellModel.Year]
                    .GraphEvidence[vertex.Key] = vertexData;
            }
            else
            {
                cellModel.Data = null;

                this.LineEvidence
                    .SectionEvidences[cellModel.SectionId]
                    .YearEvidences[cellModel.Year]
                    .GraphEvidence.Remove(vertex.Key);
            }
        }

        public void SetCell(CellModel cellModel, string newStr, string oldStr = null)
        {
            if (cellModel.IsColumnSectionId)
            {
                cellModel.Data = newStr;

                if (oldStr != null)
                {
                    var oldSectionEvidence = this.LineEvidence.SectionEvidences[oldStr];
                    var oldSectionEvidenceIndex = this.LineEvidence.SectionEvidences.IndexOf(oldSectionEvidence);
                    this.LineEvidence.SectionEvidences.Remove(oldSectionEvidence);
                    oldSectionEvidence.Id = newStr;
                    this.LineEvidence.SectionEvidences.Insert(oldSectionEvidenceIndex, oldSectionEvidence);
                }
                else
                {
                    var newSectionEvidence = new SectionEvidence
                    {
                        Id = newStr,
                    };

                    foreach (var year in this.LineEvidence.Years)
                    {
                        newSectionEvidence.YearEvidences.Add(new YearEvidence {Year = year});
                    }

                    this.LineEvidence.SectionEvidences.Add(newSectionEvidence);
                }

                return;
            }

            var selectedVertex = this.Graph.SelectedVertex;

            if (selectedVertex == null) return;

            selectedVertex.EvidenceString = newStr;
            selectedVertex.UpdateEvidence();

            this.SetCell(cellModel, selectedVertex);
        }

        private void InputGridView_AutoGeneratingColumn(object sender, GridViewAutoGeneratingColumnEventArgs e)
        {
            e.Column.CellTemplateSelector = this.InputGridView.FindResource("CellTemplateSelector") as CellTemplateSelector;
        }

        private void InputGridView_CellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {
            this.SetCell(e.Cell.ToModel(), e.NewData as string, e.OldData as string);
            this.Graph.Run();
        }

        private void InputGridView_CellValidating(object sender, GridViewCellValidatingEventArgs e)
        {
            if (e.Cell.ToModel().IsColumnSectionId) return;

            var evidenceString = e.NewValue as string;
            var vertexEvidence = EvidenceStringFactory.Create(evidenceString).Parse(this.Graph.SelectedVertex.States, evidenceString);

            if (vertexEvidence != null || evidenceString == string.Empty) { return; }
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
            if (e.Key != Key.Delete && e.Key != Key.Back) { return; }
            
            foreach (var cellInfo in this.InputGridView.SelectedCells)
            {
                this.SetCell(cellInfo.ToModel(), null);
            }
            
        }

        private void InputGridView_Pasted(object sender, RadRoutedEventArgs e)
        {
            foreach (var cellClipboardEventArg in this.cellClipboardEventArgs)
            {
                this.SetCell(cellClipboardEventArg.Cell.ToModel(), cellClipboardEventArg.Value as string, this.oldValues[cellClipboardEventArg] as string);
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