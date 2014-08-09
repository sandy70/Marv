using System.Collections.Generic;
using System.Linq;
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

        public void SetCell(CellModel cellModel, string newStr, string oldStr = null)
        {
            if (cellModel.IsColumnSectionId)
            {
                if(!string.IsNullOrWhiteSpace(newStr))
                {
                    if (!string.IsNullOrWhiteSpace(oldStr))
                    {
                        this.LineEvidence.SectionEvidences.ReplaceKey(oldStr, newStr);
                    }
                    else
                    {
                        this.LineEvidence.SectionEvidences.Add(newStr);
                    }
                }

                cellModel.Data = newStr;
                return;
            }

            var selectedVertex = this.Graph.SelectedVertex;

            if (selectedVertex == null) return;

            var values = selectedVertex.States.Parse(newStr);

            var evidence = values == null ? null : new VertexEvidence { Values = values.ToArray(), String = newStr};

            cellModel.Data = evidence;
            
            this.LineEvidence
                .SectionEvidences[cellModel.SectionId]
                .YearEvidences[cellModel.Year]
                .VertexEvidences[selectedVertex.Key] = evidence;

            this.Graph.Run(LineEvidence.SectionEvidences[cellModel.SectionId]);
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

            var vertexEvidences = this.LineEvidence.SectionEvidences[cellModel.SectionId].YearEvidences[cellModel.Year].VertexEvidences;

            this.Graph.SetEvidence(vertexEvidences);
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
                this.SetCell(cellClipboardEventArg.Cell.ToModel(), cellClipboardEventArg.Value as string, oldValues[cellClipboardEventArg] as string);
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