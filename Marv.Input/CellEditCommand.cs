using System;
using Marv.Common;
using Telerik.Windows.Controls;

namespace Marv.Input
{
    public class CellEditCommand : ICommand
    {
        public GridViewCellEditEndedEventArgs E { get; set; }

        public Vertex SelectedVertex { get; set; }

        public CellEditCommand(Vertex selVertex, GridViewCellEditEndedEventArgs e)
        {
            this.SelectedVertex = selVertex;
            this.E = e;
        }

        public void Execute()
        {
            var columnName = E.Cell.Column.UniqueName;
            var row = E.Cell.ParentRow.Item as EvidenceRow;
            DateTime dateTime;

            if (columnName.TryParse(out dateTime))
            {
                row[columnName] = this.SelectedVertex.States.ParseEvidenceString(E.NewData as string);

                this.SelectedVertex.IsUserEvidenceComplete = true;
            }

            else
            {
                row[columnName] = (double) E.NewData;
            }
        }

        public bool Undo()
        {
            var columnName = E.Cell.Column.UniqueName;
            var row = E.Cell.ParentRow.Item as EvidenceRow;

            if (row == null) // if the ParentRow is a new row, then the Item of new row returns null
            {
                return false;
            }

            row[columnName] = this.E.OldData;
            return true;
        }
    }
}