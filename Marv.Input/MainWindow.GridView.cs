using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Marv.Common;
using Telerik.Windows;
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
            var columnName = e.Cell.Column.UniqueName;
            var row = e.Cell.ParentRow.Item as EvidenceRow;
            var selectedVertex = this.Graph.SelectedVertex;

            DateTime dateTime;

            if (columnName.TryParse(out dateTime))
            {
                // This is a date time column and vertex evidence cell

                row[columnName] = selectedVertex.States.ParseEvidenceString(e.NewData as string);

                this.Plot(row, columnName);
            }
        }

        private void GridView_CellValidating(object sender, GridViewCellValidatingEventArgs e)
        {
            var columnName = e.Cell.Column.UniqueName;
            var selectedVertex = this.Graph.SelectedVertex;

            DateTime dateTime;

            if (columnName.TryParse(out dateTime))
            {
                // This is a date time column and vertex evidence cell
                var vertexEvidence = selectedVertex.States.ParseEvidenceString(e.NewValue as string);

                if (vertexEvidence.Type == VertexEvidenceType.Invalid)
                {
                    e.IsValid = false;
                    e.ErrorMessage = "Invalid evidence for node " + selectedVertex.Key;
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

            this.selectedRow = e.NewCell.ParentRow.Item as EvidenceRow;

            var gridViewColumn = gridViewCell.Column;

            this.selectedColumnName = gridViewColumn.UniqueName;

            DateTime dateTime;

            this.IsCellToolbarVisible = gridViewColumn.UniqueName.TryParse(out dateTime);

            if (this.selectedColumnName.TryParse(out dateTime))
            {
                this.Plot(this.selectedColumnName);
            }
        }

        private void GridView_Pasted(object sender, RadRoutedEventArgs e)
        {
            foreach (var pastedCell in this.pastedCells)
            {
                var evidenceRow = pastedCell.Cell.Item as EvidenceRow;
                var colName = pastedCell.Cell.Column.UniqueName;

                if (pastedCell.Value == null)
                {
                    continue;
                }

                var val = pastedCell.Value.ToString();

                DateTime dateTime;

                if (colName.TryParse(out dateTime))
                {
                    if (evidenceRow != null)
                    {
                        var vertexEvidence = this.Graph.SelectedVertex.States.ParseEvidenceString(val);

                        evidenceRow[colName] = vertexEvidence;
                    }
                }

                else
                {
                    evidenceRow[colName] = Convert.ToDouble(val);
                }
            }
            this.Validate();
            this.pastedCells.Clear();
        }

        private void GridView_PastingCellClipboardContent(object sender, GridViewCellClipboardEventArgs e)
        {
            var cellModel = e.Cell.ToModel();

            if (!e.Cancel)
            {
                this.pastedCells.Add(e);
            }
        }

        private void GridView_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            this.Maximum = this.Table.Max(row => Math.Max(row.From, row.To));
            this.Minimum = this.Table.Min(row => Math.Min(row.From, row.To));
        }

        private void GridView_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            var evidenceRow = e.Row.Item as EvidenceRow;

            e.IsValid = evidenceRow.From <= evidenceRow.To;
        }

        private bool Validate()
        {
            var selectedVertexKey = this.Graph.SelectedVertex.Key;

            var evidenceTable = this.lineDataObj[this.selectedTheme][selectedVertexKey];

            var fromToList = new List<double>();

            foreach (var evidenceRow in evidenceTable)
            {
                fromToList.Add(evidenceRow.From);
                fromToList.Add(evidenceRow.To);
            }

            for (var i = 0; i < fromToList.Count - 1; i++)
            {
                if (fromToList[i] > fromToList[i + 1])
                {
                    // style the row here (pending work)
                    MessageBox.Show("Invalid input in row " + i / 2);
                    return false;
                }
            }

            return true;
        }

        private void ValidateButton_Click(object sender, RoutedEventArgs e)
        {
            var isValid = this.Validate();
        }
    }
}