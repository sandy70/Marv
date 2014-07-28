using System;
using System.Collections.Generic;
using System.Windows.Input;
using Telerik.Windows;
using Telerik.Windows.Controls;

namespace Marv.Input
{
    public partial class MainWindow
    {
        private readonly List<GridViewCellClipboardEventArgs> cellClipboardEventArgs = new List<GridViewCellClipboardEventArgs>();
        private Dictionary<GridViewCellClipboardEventArgs, object> oldValues = new Dictionary<GridViewCellClipboardEventArgs, object>();

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

            if (this.Graph.SelectedVertex == null)
            {
                return;
            }


            if (selectedVertex == null) return;

            selectedVertex.EvidenceString = newStr;
            selectedVertex.UpdateEvidence();

            var vertexData = selectedVertex.GetData();

            cellModel.Data = vertexData;

            if (selectedVertex.IsEvidenceEntered)
            {
                this.LineEvidence
                    .SectionEvidences[cellModel.SectionId]
                    .YearEvidences[cellModel.Year]
                    .GraphEvidence[this.Graph.SelectedVertex.Key] = vertexData;
            }
            else
            {
                this.LineEvidence
                    .SectionEvidences[cellModel.SectionId]
                    .YearEvidences[cellModel.Year]
                    .GraphEvidence.Remove(this.Graph.SelectedVertex.Key);
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

        private void InputGridView_CellValidating(object sender, GridViewCellValidatingEventArgs e)
        {
            
            if (!e.Cell.ToModel().IsColumnSectionId && !(e.NewValue.Equals("")))
            {
                double d;
                bool isRange = e.NewValue.ToString().Contains(":");
                if (!(isRange) && !(Double.TryParse(e.NewValue.ToString(), out d)))
                {
                    e.IsValid = false;
                }
                else if (isRange)
                {
                    var valueSet = e.NewValue.ToString().Split(':');
                    if (valueSet.Length > 2)
                    {
                        e.IsValid = false;                        
                    }
                    else if(!(Double.TryParse(valueSet[0], out d)) || !(Double.TryParse(valueSet[1], out d)))
                    {
                        e.IsValid = false;                        
                    }
                    else if (Convert.ToDouble(valueSet[0]) > Convert.ToDouble(valueSet[1]))
                    {
                        e.IsValid = false;
                    }
                }
            }
            if (!e.IsValid)
            {
                e.ErrorMessage = "Not a proper value or range of values.";
            }
        }

        private void InputGridView_CurrentCellChanged(object sender, GridViewCurrentCellChangedEventArgs e)
        {
            if (e.NewCell == null) return;

            var cellModel = e.NewCell.ToModel();

            if (cellModel.IsColumnSectionId)
            {
                return;
            }

            if (cellModel.IsColumnSectionId) return;

            var sectionEvidence = this.LineEvidence.SectionEvidences[cellModel.SectionId];

            if (!sectionEvidence.YearEvidences.ContainsKey(cellModel.Year))
            {
                sectionEvidence.YearEvidences.Add(new YearEvidence {Year = cellModel.Year});
            }

            this.Graph.SetEvidence(sectionEvidence.YearEvidences[cellModel.Year].GraphEvidence);
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
                this.SetCell(cellClipboardEventArg.Cell.ToModel(), cellClipboardEventArg.Value as string, this.oldValues[cellClipboardEventArg] as string);
            }

            cellClipboardEventArgs.Clear();
        }

        private void InputGridView_PastingCellClipboardContent(object sender, GridViewCellClipboardEventArgs e)
        {
            this.cellClipboardEventArgs.Add(e);
            this.oldValues[e] = e.Cell.ToModel().Data;
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