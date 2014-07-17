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

        private bool checkValidityOfInput(string str)
        {
            double value;
            if ((!str.Contains(":")))
            {
                if (Double.TryParse(str, out value))
                {
                    return true;
                }
            }
            else if (str.Split(":".ToCharArray()).Length == 2)
            {
                String[] valueSet = str.Split(":".ToCharArray());
                if (Double.TryParse(valueSet[0], out value) && Double.TryParse(valueSet[1], out value))
                {
                    return true;
                }
            }
            else if(str == null){
                return true;
            }
            return false;
        }

        public void SetCell(dynamic row, string columnHeader, string str)
        {
            if (checkValidityOfInput(str) || columnHeader.Equals("Section ID"))
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
                var evidence = new VertexEvidence(this.Graph.SelectedVertex.Evidence, this.Graph.SelectedVertex.EvidenceString);
                    row[columnHeader] = evidence;
                this.LineEvidence[sectionId, Convert.ToInt32(columnHeader), this.Graph.SelectedVertex.Key] = evidence;
                }
            }
            else
            {
                row[columnHeader] = null;
                this.Notifications.Add(new NotificationIndeterminate
                {
                    Description = "Please enter valid data (number, range in the form number:number).",
                    Name = "Invalid Data Entry"
                    
                });
               
            }
            
        }

        private void InputGridView_AutoGeneratingColumn(object sender, GridViewAutoGeneratingColumnEventArgs e)
        {
            e.Column.CellTemplateSelector = this.InputGridView.FindResource("CellTemplateSelector") as CellTemplateSelector;
        }

        private void InputGridView_CellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {
            this.SetCell(e.Cell.DataContext as dynamic, e.Cell.Column.Header as string, e.NewData as string);
        }

        private void InputGridView_CurrentCellChanged(object sender, GridViewCurrentCellChangedEventArgs e)
        {
            if (e.NewCell == null) return;

            var cellModel = e.NewCell.ToModel();

            this.Graph.SetEvidence(this.LineEvidence[cellModel.SectionId, cellModel.Year]);
            this.Graph.Run();
        }

        private void InputGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete || e.Key == Key.Back)
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