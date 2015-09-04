using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Marv.Common;
using Marv.Common.Types;
using Telerik.Charting;
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

            if (this.SelectedVertex != null)
            {
                this.UpdateTable();
            }
        }

        private void Ellipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.DraggedPoint = null;
        }

        private void GridView_AddingNewDataItem(object sender, GridViewAddingNewEventArgs e)
        {
            e.OwnerGridViewItemsControl.CurrentColumn = e.OwnerGridViewItemsControl.Columns[0];
            this.CreatedRowsCount++;
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
            var vertexEvidence = this.selectedVertex.States.ParseEvidenceString(e.NewData as string);

            var command = new CellEditCommand(row, columnName, this.SelectedVertex, e.NewData, e.OldData);
            command.Execute();

            this.UpdateCommandStack(command);

            if (vertexEvidence.Type != VertexEvidenceType.Invalid)
            {
                this.Plot(row, columnName);
            }
        }

        private void GridView_CellValidating(object sender, GridViewCellValidatingEventArgs e)
        {
            var columnName = e.Cell.Column.UniqueName;

            DateTime dateTime;

            if (columnName.TryParse(out dateTime))
            {
                // This is a date time column and vertex evidence cell
                var vertexEvidence = this.SelectedVertex.States.ParseEvidenceString(e.NewValue as string);

                if (vertexEvidence.Type == VertexEvidenceType.Invalid)
                {
                    e.IsValid = false;
                    e.ErrorMessage = "Invalid evidence for node " + this.SelectedVertex.Key;
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

            this.SelectedColumnName = gridViewColumn.UniqueName;

            this.SelectedInterpolationData = this.interpolationData[this.SelectedVertex.Key][this.SelectedColumnName];

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
            var newRowsCount = this.AddRowCommandsCount;
            var command = new PasteCommand(this.SelectedVertex, this.pastedCells, oldValues, (newRowsCount - this.CreatedRowsCount), this.Table);

            command.Execute();

            this.oldValues.Clear();

            this.AddRowCommandsCount = 0;
            this.CreatedRowsCount = 0;

            this.UpdateCommandStack(command);
            this.SelectedVertex.IsUserEvidenceComplete = true;

            this.pastedCells.Clear();

            if (this.Table.Count != 0)
            {
                if (this.isBaseTableAvailable)
                {
                    this.Maximum = Math.Max(this.Table.Max(row => Math.Max(row.From, row.To)), this.BaseTableMax);
                    this.Minimum = Math.Min(this.Table.Min(row => Math.Min(row.From, row.To)), this.BaseTableMin);
                }
                else
                {
                    this.Maximum = this.Table.Max(row => Math.Max(row.From, row.To));
                    this.Minimum = this.Table.Min(row => Math.Min(row.From, row.To));
                }
            }
            this.selectedColumnName = this.Table.DateTimes.First().String();
            this.Plot(this.selectedColumnName);
        }

        private void GridView_PastingCellClipboardContent(object sender, GridViewCellClipboardEventArgs e)
        {
            if (!e.Cancel)
            {
                this.pastedCells.Add(e);

                var colname = e.Cell.Column.UniqueName;
                var row = e.Cell.Item as EvidenceRow;
                var oldValue = row[colname];

                this.oldValues.Add(oldValue);
            }
        }

        private void GridView_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            if (this.isBaseTableAvailable)
            {
                this.Maximum = Math.Max(this.Table.Max(row => Math.Max(row.From, row.To)), this.BaseTableMax);
                this.Minimum = Math.Min(this.Table.Min(row => Math.Min(row.From, row.To)), this.BaseTableMin);
            }
            else
            {
                this.Maximum = this.Table.Max(row => Math.Max(row.From, row.To));
                this.Minimum = this.Table.Min(row => Math.Min(row.From, row.To));
            }
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (this.CurrentCommand < 0)
            {
                return;
            }

            var command = this.commandStack[this.CurrentCommand];

            if (command.Undo())
            {
                this.commandStack.Remove(command);
                this.CurrentCommand = this.commandStack.Count - 1;
            }

            if (this.CurrentCommand < 0)
            {
                this.SelectedVertex.IsUserEvidenceComplete = false;
            }

            var columnName = this.CurrentColumn == null ? this.Table.DateTimes.First().String() : this.CurrentColumn.UniqueName;

            this.Plot(columnName);
        }
    }
}