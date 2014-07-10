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

        public void SetCell(dynamic row, string columnHeader, string str)
        {
            this.SelectedVertex.EvidenceString = str;
            this.SelectedVertex.UpdateEvidence();

            var sectionId = row["Section ID"] as string;

            if (columnHeader == "Section ID")
            {
                row[columnHeader] = str;
            }
            else
            {
                var evidence = new VertexEvidence(this.SelectedVertex.Evidence, this.SelectedVertex.EvidenceString);
                row[columnHeader] = evidence;
                this.LineEvidence[sectionId, Convert.ToInt32(columnHeader), this.SelectedVertex.Key] = evidence;
            }
        }

        private void InputGridView_AutoGeneratingColumn(object sender, GridViewAutoGeneratingColumnEventArgs e)
        {
            e.Column.CellTemplateSelector = this.InputGridView.FindResource("CellTemplateSelector") as CellTemplateSelector;
        }

        private void InputGridView_CellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {
            var row = e.Cell.ParentRow.DataContext as Dynamic;

            if (e.Cell.Column.DisplayIndex <= 0)
            {
                row["Section ID"] = e.NewData;
                return;
            }

            this.SelectedVertex.EvidenceString = e.NewData as string;
            this.SelectedVertex.UpdateEvidence();
            this.UpdateModelEvidence();

            var year = (string) e.Cell.Column.Header;
            row[year] = new VertexEvidence(this.SelectedVertex.Evidence, this.SelectedVertex.EvidenceString);
        }

        private void InputGridView_CurrentCellChanged(object sender, GridViewCurrentCellChangedEventArgs e)
        {
            if (e.NewCell != null)
            {
                this.GraphControl.IsEnabled = true;
                this.VertexControl.IsEnabled = true;

                try
                {
                    var row = e.NewCell.ParentRow.DataContext as Dynamic;

                    if (row != null)
                    {
                        var sectionId = row["Section ID"] as string;
                        var year = Convert.ToInt32((string) e.NewCell.Column.Header);

                        var evidence = this.LineEvidence[sectionId, year];

                        this.Graph.SetEvidence(evidence);
                    }

                    this.Graph.Run();
                }
                catch (FormatException)
                {
                    this.VertexControl.IsEnabled = false;
                    this.GraphControl.IsEnabled = false;
                }
            }
            else
            {
                this.GraphControl.IsEnabled = false;
                this.VertexControl.IsEnabled = false;
            }
        }

        private void InputGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                foreach (var cellInfo in this.InputGridView.SelectedCells)
                {
                    this.SetCell(cellInfo.Item as dynamic, cellInfo.Column.Header as string, null);
                }
            }
        }

        private void InputGridView_Pasted(object sender, RadRoutedEventArgs e)
        {
            foreach (var cellClipboardEventArg in this.cellClipboardEventArgs)
            {
                this.SetCell(cellClipboardEventArg.Cell.Item as dynamic, cellClipboardEventArg.Cell.Column.Header as string, cellClipboardEventArg.Value as string);
            }

            cellClipboardEventArgs.Clear();
        }

        private void InputGridView_PastingCellClipboardContent(object sender, GridViewCellClipboardEventArgs e)
        {
            this.cellClipboardEventArgs.Add(e);
        }
    }
}