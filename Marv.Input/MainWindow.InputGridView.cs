using System;
using System.Collections.Generic;
using System.Windows.Input;
using Marv.Common;
using Marv.Common.Graph;
using Telerik.Windows;
using Telerik.Windows.Controls;

namespace Marv.Input
{
    public partial class MainWindow
    {
        private readonly List<GridViewCellClipboardEventArgs> cellClipboardEventArgs = new List<GridViewCellClipboardEventArgs>();

        public void SetCell(CellModel cellModel, string newStr, string oldStr = null)
        {
            if (cellModel.IsColumnSectionId)
            {
                cellModel.Data = newStr;

                if (oldStr != null)
                {
                    this.LineEvidence[newStr] = this.LineEvidence[oldStr];
                } 

                return;
            }

            var selectedVertex = this.Graph.SelectedVertex;

            if (selectedVertex == null) return;

            selectedVertex.EvidenceString = newStr;
            selectedVertex.UpdateEvidence();

            var vertexData = selectedVertex.GetData();

            cellModel.Data = vertexData;

            if (selectedVertex.IsEvidenceEntered)
            {
                this.LineEvidence[cellModel.SectionId, cellModel.Year, selectedVertex.Key] = vertexData;
            }
            else
            {
                this.LineEvidence.Remove(cellModel.SectionId, cellModel.Year, selectedVertex.Key);
            }
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

        private void InputGridView_CurrentCellChanged(object sender, GridViewCurrentCellChangedEventArgs e)
        {
            if (e.NewCell == null) return;

            var cellModel = e.NewCell.ToModel();

            if (cellModel.IsColumnSectionId) return;

            this.Graph.SetEvidence(this.LineEvidence[cellModel.SectionId, cellModel.Year]);
            this.Graph.Run();
        }

        private void InputGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                foreach (var cellInfo in this.InputGridView.SelectedCells)
                {
                    this.SetCell(cellInfo.ToModel(), null);
                }
            }
        }

        private void InputGridView_Pasted(object sender, RadRoutedEventArgs e)
        {
            foreach (var cellClipboardEventArg in this.cellClipboardEventArgs)
            {
                this.SetCell(cellClipboardEventArg.Cell.ToModel(), cellClipboardEventArg.Value as string);
                var cellModel = cellClipboardEventArg.Cell.ToModel();
                var str = cellClipboardEventArg.Value as string;

                this.SetCell(cellModel, str);
            }

            cellClipboardEventArgs.Clear();
        }

        private void InputGridView_PastingCellClipboardContent(object sender, GridViewCellClipboardEventArgs e)
        {
            this.cellClipboardEventArgs.Add(e);
        }

        private bool checkValidityOfInput(string str)
        {
            double value;
            if (str != null && (!str.Contains(":")))
            {
                if (Double.TryParse(str, out value))
                {
                    return true;
                }
            }
            else if (str.Split(":".ToCharArray()).Length == 2)
            {
                var valueSet = str.Split(":".ToCharArray());
                if (Double.TryParse(valueSet[0], out value) && Double.TryParse(valueSet[1], out value))
                {
                    return true;
                }
            }
            else if (str == null)
            {
                return true;
            }
            return false;
        }
    }
}