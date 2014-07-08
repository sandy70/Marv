using System;
using Marv.Common;
using Marv.Common.Graph;
using Telerik.Windows;
using Telerik.Windows.Controls;

namespace Marv.Input
{
    public partial class MainWindow
    {
        private GridViewCellClipboardEventArgs cellClipboardEventArgs;

        private void InputGridView_AutoGeneratingColumn(object sender, GridViewAutoGeneratingColumnEventArgs e)
        {
            e.Column.CellTemplateSelector = this.InputGridView.FindResource("CellTemplateSelector") as CellTemplateSelector;
        }

        private void InputGridView_CellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {
            if (e.Cell.Column.DisplayIndex <= 0) return;

            this.SelectedVertex.EvidenceString = e.NewData as string;
            this.SelectedVertex.UpdateEvidence();
            this.UpdateModelEvidence();

            var row = e.Cell.ParentRow.DataContext as Dynamic;
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

                        var evidence = this.LineEvidence.GetValueOrNew(sectionId, year);

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

        private void InputGridView_Pasted(object sender, RadRoutedEventArgs e)
        {
            if (this.cellClipboardEventArgs != null)
            {
                this.SelectedVertex.EvidenceString = cellClipboardEventArgs.Value as string;
                this.SelectedVertex.UpdateEvidence();
                this.UpdateModelEvidence();

                var row = cellClipboardEventArgs.Cell.Item as Dynamic;
                var year = (string) cellClipboardEventArgs.Cell.Column.Header;
                row[year] = new VertexEvidence(this.SelectedVertex.Evidence, this.SelectedVertex.EvidenceString);

                this.cellClipboardEventArgs = null;
            }
        }

        private void InputGridView_PastingCellClipboardContent(object sender, GridViewCellClipboardEventArgs e)
        {
            this.cellClipboardEventArgs = e;
        }
    }
}