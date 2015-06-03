using System;
using System.Data;
using System.Windows.Controls;
using Marv.Common;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

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

            // If this is a DateTime column
            if (columnName.TryParse(out dateTime))
            {
                this.Plot((e.Cell.ParentRow.DataContext as DataRowView).Row, columnName);
            }
        }

        private void GridView_CellValidating(object sender, GridViewCellValidatingEventArgs e)
        {
            var row = (e.Row.Item as DataRowView).Row;
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
            else
            {
                // this is a location cell
                if (e.NewValue.GetType() != this.Table.Columns[columnName].DataType)
                {
                    e.IsValid = false;
                    e.ErrorMessage = "Invalid value for column " + columnName;
                }
            }
        }

        private void GridView_CurrentCellChanged(object sender, GridViewCurrentCellChangedEventArgs e)
        {
            if (e.NewCell == null)
            {
                return;
            }

            DateTime dateTime;

            if (e.NewCell.Column.UniqueName.TryParse(out dateTime))
            {
                // this is a date time column
                this.IsCellToolbarVisible = true;
            }
            else
            {
                this.IsCellToolbarVisible = false;
            }
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
            this.Maximum = this.Table.GetMaximum();
            this.Minimum = this.Table.GetMinimum();
        }

        private void GridView_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            Console.WriteLine("GridView_RowValidating");
            e.IsValid = this.Table.IsValid((e.Row.Item as DataRowView).Row);
        }
    }
}