using System;
using System.Collections.Generic;
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

            DateTime dateTime;

            if (columnName.TryParse(out dateTime))
            {
                // This is a date time column and vertex evidence cell
                var vertexEvidence = this.SelectedVertex.States.ParseEvidenceString(e.NewData as string);
                if (vertexEvidence.Type != VertexEvidenceType.Invalid)
                {
                    row[columnName] = vertexEvidence;
                    this.Plot(row, columnName);
                    this.SelectedVertex.IsUserEvidenceComplete = true;
                }
                
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

            this.selectedColumnName = gridViewColumn.UniqueName;

            DateTime dateTime;

            this.IsCellToolbarEnabled = gridViewColumn.UniqueName.TryParse(out dateTime);

            if (this.selectedColumnName.TryParse(out dateTime))
            {
                if (this.UserNumberPoints != null && this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName] != null)
                {
                    this.CurrentInterpolatorDataPoints = this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName];
                }

                    //else
                    //{
                    //    this.CurrentInterpolatorDataPoints = null;
                    //}
                else
                {
                    this.CurrentInterpolatorDataPoints.MaxNumberPoints.Clear();
                    this.CurrentInterpolatorDataPoints.ModeNumberPoints.Clear();
                    this.CurrentInterpolatorDataPoints.MinNumberPoints.Clear();

                    this.CurrentInterpolatorDataPoints.MaxNumberPoints.Add(new ScatterDataPoint());
                    this.CurrentInterpolatorDataPoints.ModeNumberPoints.Add(new ScatterDataPoint());
                    this.CurrentInterpolatorDataPoints.MinNumberPoints.Add(new ScatterDataPoint());
                }

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

                if (colName.Equals(val))
                {
                    continue;
                }

                if (colName == "From")
                {
                    evidenceRow.From = Convert.ToDouble(val);
                }

                if (colName == "To")
                {
                    evidenceRow.To = Convert.ToDouble(val);
                }
                DateTime dateTime;

                if (colName.TryParse(out dateTime))
                {
                    if (evidenceRow != null)
                    {
                        var vertexEvidence = this.SelectedVertex.States.ParseEvidenceString(val);
                        evidenceRow[colName] = vertexEvidence;
                        
                    }
                }

                else
                {
                    evidenceRow[colName] = Convert.ToDouble(val);
                }
            }
            this.SelectedVertex.IsUserEvidenceComplete = true;
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

        private void Intepolate_Click(object sender, RoutedEventArgs e)
        {
            this.IsInterpolateClicked = true;

            if (this.SelectedVertex.Type == VertexType.Labelled || this.SelectedVertex.Type == VertexType.Boolean)
            {
                return;
            }
            if (this.UserNumberPoints == null)
            {
                this.UserNumberPoints = new Dict<string, string, InterpolatorDataPoints>();
            }

            var vertexAvailable = this.UserNumberPoints.Keys.Any(key => key.Equals(this.SelectedVertex.Key));

            if (!vertexAvailable)
            {
                var dateColumns = new Dict<string, InterpolatorDataPoints>();

                foreach (var dateTime in this.dates)
                {
                    dateColumns.Add(dateTime.String(), new InterpolatorDataPoints());
                }
                this.UserNumberPoints.Add(this.SelectedVertex.Key, dateColumns);
            }

            this.CurrentInterpolatorDataPoints = this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName];

            var minMaxValues = this.lineDataObj[DataTheme.User][this.SelectedVertex.Key].GetMinMaxUserValues(this.selectedColumnName);

            this.PlotInterpolatorLines(minMaxValues);

         }

        private void PlotInterpolatorLines(Dict<string, double> minMaxValues)
        {
            this.MinUserValue = minMaxValues["Minimum"];
            this.MaxUserValue = minMaxValues["Maximum"];

            var maxLineStartPoint = new ScatterDataPoint
            {
                XValue = this.Minimum,
                YValue = this.MaxUserValue
            };
            var maxLineEndPoint = new ScatterDataPoint
            {
                XValue = this.Maximum,
                YValue = this.MaxUserValue
            };

            if (this.UserNumberPoints != null && this.UserNumberPoints[this.SelectedVertex.Key] != null)
            {
                this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].MaxNumberPoints.Clear();
                this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].MaxNumberPoints.Add(maxLineStartPoint);
                this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].MaxNumberPoints.Add(maxLineEndPoint);
            }

            var modeLineStartPoint = new ScatterDataPoint
            {
                XValue = this.Minimum,
                YValue = (this.MaxUserValue + this.MinUserValue) / 2,
            };
            var modeLineEndPoint = new ScatterDataPoint
            {
                XValue = this.Maximum,
                YValue = (this.MaxUserValue + this.MinUserValue) / 2,
            };

            if (this.UserNumberPoints != null && this.UserNumberPoints[this.SelectedVertex.Key] != null)
            {
                this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].ModeNumberPoints.Clear();
                this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].ModeNumberPoints.Add(modeLineStartPoint);
                this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].ModeNumberPoints.Add(modeLineEndPoint);
            }

            var minLineStartPoint = new ScatterDataPoint
            {
                XValue = this.Minimum,
                YValue = this.MinUserValue,
            };
            var minLineEndPoint = new ScatterDataPoint
            {
                XValue = this.Maximum,
                YValue = this.MinUserValue,
            };

            if (this.UserNumberPoints == null || this.UserNumberPoints[this.SelectedVertex.Key] == null)
            {
                return;
            }

            this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].MinNumberPoints.Clear();
            this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].MinNumberPoints.Add(minLineStartPoint);
            this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].MinNumberPoints.Add(minLineEndPoint);
        }

        private void Validate()
        {
            var selectedVertexKey = this.SelectedVertex.Key;

            var evidenceTable = this.lineDataObj[this.selectedTheme][selectedVertexKey];

            var fromToList = new List<double>();

            foreach (var evidenceRow in evidenceTable)
            {
                fromToList.Add(evidenceRow.From);
                fromToList.Add(evidenceRow.To);
            }

            foreach (var pastedCell in this.pastedCells) {}

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