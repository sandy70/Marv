using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Marv.Common;
using Marv.Common.Types;
using Marv.Controls;
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

            else if (! (this.BaseTableMin<= (double)e.NewValue && (double)e.NewValue <= this.BaseTableMax))
            {
                e.IsValid = false;
                e.ErrorMessage = "sections should be within pipeline length";
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

            DateTime dateTime;

            this.IsCellToolbarEnabled = gridViewColumn.UniqueName.TryParse(out dateTime);

            if (this.selectedColumnName.TryParse(out dateTime))
            {
                if (this.UserNumberPoints != null && this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName] != null)
                {
                    this.CurrentInterpolatorDataPoints = this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName];
                }

                    //else
                    // {
                    //            this.CurrentInterpolatorDataPoints = null;
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
            var newRowsCount = this.AddRowCommandsCount;
            var command = new PasteCommand(this.SelectedVertex, this.pastedCells, oldValues, (newRowsCount - this.CreatedRowsCount), this.Table);

            command.Execute();

            this.oldValues.Clear();

            this.AddRowCommandsCount = 0;
            this.CreatedRowsCount = 0;

            this.UpdateCommandStack(command);
            this.SelectedVertex.IsUserEvidenceComplete = true;

            this.pastedCells.Clear();

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
            
        }
        
        private void Interpolate()
        {
            if (this.SelectedVertex == null )
            {
                return;
            }

            if (this.SelectedColumnName ==null)
            {
                foreach (var button in this.InterpolationToolBar.GetChildren<RadioButton>())
                {
                    button.IsChecked = false;
                }
                return;
            }
            if (this.SelectedVertex.Type == VertexType.Labelled || this.SelectedVertex.Type == VertexType.Boolean)
            {

                MessageBox.Show("cannot interpolate data for labelled or boolean");
                return;
            }

            this.ClearInterpolatorLines();
     
            this.IsInterpolateClicked = true;
            
            if (this.UserNumberPoints == null)
            {
                this.UserNumberPoints = new Dict<string, string, IInterpolatorDataPoints>();
            }

            var vertexAvailable = this.UserNumberPoints.Keys.Any(key => key.Equals(this.SelectedVertex.Key));

            if (!vertexAvailable)
            {
                var dateColumns = new Dict<string, IInterpolatorDataPoints>();

                foreach (var dateTime in this.dates)
                {
                    if (this.InterpolatorDistribution.Equals(DistributionType.SingleValue))
                    {
                        dateColumns.Add(dateTime.String(), new SingleValueInterpolator());
                    }
                    else if (this.InterpolatorDistribution.Equals(DistributionType.Uniform))
                    {
                        dateColumns.Add(dateTime.String(), new UniformInterpolator());
                    }
                    else
                    {
                        dateColumns.Add(dateTime.String(), new TriangularInterpolator());
                    }
                }
                this.UserNumberPoints.Add(this.SelectedVertex.Key, dateColumns);
            }
            else
            {
                var linesForColAvailable = this.UserNumberPoints[this.SelectedVertex.Key].Keys.Any(columnName => columnName.Equals(this.SelectedColumnName));

                if (!linesForColAvailable)
                {
                    IInterpolatorDataPoints interpolatorLine = null;
                    if (this.InterpolatorDistribution.Equals(DistributionType.SingleValue))
                    {
                        interpolatorLine = new SingleValueInterpolator();
                    }
                    else if (this.InterpolatorDistribution.Equals(DistributionType.Uniform))
                    {
                        interpolatorLine = new UniformInterpolator();
                    }
                    else
                    {
                        interpolatorLine = new TriangularInterpolator();
                    }

                    this.UserNumberPoints[this.SelectedVertex.Key][this.SelectedColumnName] = interpolatorLine;
                }
                
            }

            
            this.CurrentInterpolatorDataPoints = this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName];

          this.PlotInterpolatorLines();
        }

        private void ClearInterpolatorLines()
        {
            if (this.UserNumberPoints == null)
            {
                return;
            }

            this.CurrentInterpolatorDataPoints = Utils.UpdateCurrentInterpolator(this.InterpolatorDistribution);
           
            var vertexAvail = this.UserNumberPoints.Keys.Any(key => key.Equals(this.SelectedVertex.Key));
            if (vertexAvail)
            {
                this.UserNumberPoints[this.SelectedVertex.Key].Remove(this.SelectedColumnName);
                                        
            }

            if (this.UserNumberPoints[this.SelectedVertex.Key].Count == 0)
            {
                this.UserNumberPoints.Remove(this.SelectedVertex.Key);
            }

           }

        private void PlotInterpolatorLines()
        {
            this.CurrentInterpolatorDataPoints.IsLineCross = false;

            
            if (this.UserNumberPoints == null || this.UserNumberPoints[this.SelectedVertex.Key] == null)
            {
                return;
            }

            var modeLineStartPoint = new ScatterDataPoint
            {
                XValue = this.BaseTableMin,
                YValue = this.SelectedVertex.GetInterpolatorPosition(Utils.ModeInterpolatorLine),
            };
            var modeLineEndPoint = new ScatterDataPoint
            {
                XValue = this.BaseTableMax,
                YValue = this.SelectedVertex.GetInterpolatorPosition(Utils.ModeInterpolatorLine),
            };

            var minLineStartPoint = new ScatterDataPoint
            {
                XValue = this.BaseTableMin,
                YValue = this.SelectedVertex.GetInterpolatorPosition(Utils.MinInterpolatorLine),
            };

            var minLineEndPoint = new ScatterDataPoint
            {
                XValue = this.BaseTableMax,
                YValue = this.SelectedVertex.GetInterpolatorPosition(Utils.MinInterpolatorLine),
            };
            var maxLineStartPoint = new ScatterDataPoint
            {
                XValue = this.BaseTableMin,
                YValue = this.SelectedVertex.GetInterpolatorPosition(Utils.MaxInterpolatorLine),
            };
            var maxLineEndPoint = new ScatterDataPoint
            {
                XValue = this.BaseTableMax,
                YValue = this.SelectedVertex.GetInterpolatorPosition(Utils.MaxInterpolatorLine),
            };

           

            if (this.UserNumberPoints == null || this.UserNumberPoints[this.SelectedVertex.Key] == null)
            {
                return;
            }

            if (this.InterpolatorDistribution.Equals(DistributionType.SingleValue))
            {
                this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].ModeNumberPoints.Clear();
                this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].ModeNumberPoints.Add(modeLineStartPoint);
                this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].ModeNumberPoints.Add(modeLineEndPoint);
            }

            else if (this.InterpolatorDistribution.Equals(DistributionType.Uniform))
            {
                this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].MaxNumberPoints.Clear();
                this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].MaxNumberPoints.Add(maxLineStartPoint);
                this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].MaxNumberPoints.Add(maxLineEndPoint);

                this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].MinNumberPoints.Clear();
                this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].MinNumberPoints.Add(minLineStartPoint);
                this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].MinNumberPoints.Add(minLineEndPoint);
            }

            else
            {
                this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].MaxNumberPoints.Clear();
                this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].MaxNumberPoints.Add(maxLineStartPoint);
                this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].MaxNumberPoints.Add(maxLineEndPoint);

                this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].MinNumberPoints.Clear();
                this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].MinNumberPoints.Add(minLineStartPoint);
                this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].MinNumberPoints.Add(minLineEndPoint);

                this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].ModeNumberPoints.Clear();
                this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].ModeNumberPoints.Add(modeLineStartPoint);
                this.UserNumberPoints[this.SelectedVertex.Key][this.selectedColumnName].ModeNumberPoints.Add(modeLineEndPoint);
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