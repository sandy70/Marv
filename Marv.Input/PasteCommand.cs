using System;
using System.Collections.Generic;
using Marv.Common;
using Telerik.Windows.Controls;

namespace Marv.Input
{
    internal class PasteCommand : ICommand
    {
        public List<CellEditCommand> CellEditCommandsList = new List<CellEditCommand>();
        public int NewPastedRowsCount;
        public List<GridViewCellClipboardEventArgs> PastedCells { get; set; }
        public List<Object> PreviousValues { get; set; }
        public Vertex SelectedVertex { get; set; }
        public EvidenceTable Table { get; set; }

        public PasteCommand(Vertex selVertex, List<GridViewCellClipboardEventArgs> pastedCells, List<Object> oldValues, int newPastedRowsCount, EvidenceTable table)
        {
            this.SelectedVertex = selVertex;
            this.PastedCells = pastedCells;
            this.PreviousValues = oldValues;
            this.NewPastedRowsCount = newPastedRowsCount;
            this.Table = table;
        }

        public void Execute()
        {
            foreach (var pastedCell in PastedCells)
            {
                var evidenceRow = pastedCell.Cell.Item as EvidenceRow;
                var colName = pastedCell.Cell.Column.UniqueName;
                var newValue = pastedCell.Value;
                var oldValue = PreviousValues[PastedCells.IndexOf(pastedCell)];

                var cellEditCommand = new CellEditCommand(evidenceRow, colName, SelectedVertex, newValue, oldValue);
                CellEditCommandsList.Add(cellEditCommand);
                cellEditCommand.Execute();
            }
        }

        public bool Undo()
        {
            var isUndoSuccess = true;

            while (NewPastedRowsCount > 0 && this.Table.Count >0 )
            {
                this.Table.RemoveAt(this.Table.Count - 1);
                NewPastedRowsCount--;
            }

            foreach (var cellEditCommand in this.CellEditCommandsList)
            {
                if (this.Table.Contains(cellEditCommand.Row))
                {
                    isUndoSuccess = isUndoSuccess && cellEditCommand.Undo();
                }
            }

            if (Table.Count == 0)
            {
                this.SelectedVertex.IsUserEvidenceComplete = false;
            }
            return isUndoSuccess;
        }
    }
}