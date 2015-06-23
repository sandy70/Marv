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
        private void DataThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.IsGridViewReadOnly = !this.SelectedTheme.Equals(DataTheme.User);

            if (this.Graph != null)
            {
                this.UpdateTable();
            }
        }

        private void GridView_AddingNewDataItem(object sender, GridViewAddingNewEventArgs e)
        {
            e.OwnerGridViewItemsControl.CurrentColumn = e.OwnerGridViewItemsControl.Columns[0];
        }

        private void GridView_AutoGeneratingColumn(object sender, GridViewAutoGeneratingColumnEventArgs e)
        {
            var headerString = e.Column.Header as string;

            e.Column.CellTemplateSelector = (CellTemplateSelector) this.FindResource("CellTemplateSelector");

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

            this.IsCellToolbarEnabled = gridViewColumn.UniqueName.TryParse(out dateTime);

            if (this.selectedColumnName.TryParse(out dateTime))
            {
                this.Chart.Annotations.Remove(annotation => true);
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
            evidenceRow.IsValid = e.IsValid;
        }

        private void Validate()
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
                evidenceTable[i / 2].IsValid = !(fromToList[i] > fromToList[i + 1]);
            }
        }

        private void ValidateButton_Click(object sender, RoutedEventArgs e)
        {
            this.Validate();
        }
    }
}