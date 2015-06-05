using System;
using System.Data;
using System.Windows.Controls;
using Marv.Common;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using System.Linq;

namespace Marv.Input
{
    public partial class MainWindow
    {
        private void GridView_AddingNewDataItem(object sender, GridViewAddingNewEventArgs e)
        {
            e.OwnerGridViewItemsControl.CurrentColumn = e.OwnerGridViewItemsControl.Columns[0];
        }

        private void GridView_AutoGeneratingColumn(object sender, GridViewAutoGeneratingColumnEventArgs e)
        {
            var headerString = e.Column.Header as string;

            DateTime dateTime;

            if (headerString.TryParse(out dateTime))
            {
                e.Column.Header = new TextBlock
                {
                    Text = dateTime.ToShortDateString()
                };
            }
        }

        private void GridView_CellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {
            DateTime dateTime;

            var columnName = e.Cell.Column.UniqueName;
            var row = e.Cell.ParentRow.Item as EvidenceRow;

            // If this is a DateTime column
            if (columnName.TryParse(out dateTime))
            {
                this.Graph.SelectedVertex.SetEvidence(row[columnName] as VertexEvidence);
                this.Plot(row, columnName);
            }
        }

        private void GridView_CellValidating(object sender, GridViewCellValidatingEventArgs e)
        {
            var row = e.Row.Item as EvidenceRow;
            var columnName = e.Cell.Column.UniqueName;
            var selectedVertex = this.Graph.SelectedVertex;

            DateTime dateTime;

            if (columnName.TryParse(out dateTime))
            {
                // This is a vertex evidence cell
                var vertexEvidence = selectedVertex.States.ParseEvidenceString(e.NewValue as string);

                if (vertexEvidence.Type == VertexEvidenceType.Invalid)
                {
                    e.IsValid = false;
                    e.ErrorMessage = "Invalid evidence for node " + selectedVertex.Key;
                }
                else
                {
                    row[columnName] = vertexEvidence;
                }
            }
        }

        private void GridView_CurrentCellChanged(object sender, GridViewCurrentCellChangedEventArgs e)
        {
            var gridViewCell = e.NewCell;

            if (gridViewCell == null)
            {
                return;
            }

            var gridViewColumn = gridViewCell.Column;

            this.selectedColumnName = gridViewColumn.UniqueName;

            DateTime dateTime;

            this.IsCellToolbarVisible = gridViewColumn.UniqueName.TryParse(out dateTime);
        }

        private void GridView_PastingCellClipboardContent(object sender, GridViewCellClipboardEventArgs e)
        {
            var dataRowView = e.Cell.Item as DataRowView;
            var colName = e.Cell.Column.UniqueName;

            if (e.Value != null)
            {
                var val = e.Value.ToString();

                if (colName != "From" && colName != "To")
                {
                    if (dataRowView.Row != null)
                    {
                        var vertexEvidence = this.Graph.SelectedVertex.States.ParseEvidenceString(val);
                        dataRowView.Row[colName] = vertexEvidence;
                    }
                }
            }
        }

        private void GridView_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            // this.Maximum = this.Table.Max(row => Math.Max(row.From, row.To));
            // this.Minimum = this.Table.Min(row => Math.Max(row.From, row.To));
        }

        private void GridView_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            Console.WriteLine("GridView_RowValidating");
            
            var evidenceRow = (e.Row.Item as EvidenceRow);

            if (evidenceRow.From != null && evidenceRow.To != null)
            {
                e.IsValid = evidenceRow.From <= evidenceRow.To;
            }
            else
            {
                e.IsValid = true;
            }
        }
    }
}