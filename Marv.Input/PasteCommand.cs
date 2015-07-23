using System;
using System.Collections.Generic;
using Marv.Common;
using Telerik.Windows.Controls;

namespace Marv.Input
{
    internal class PasteCommand : ICommand
    {
        public List<CellEditCommand> CellEditCommandsList = new List<CellEditCommand>();
        public List<GridViewCellClipboardEventArgs> PastedCells { get; set; }
        public List<Object> PreviousValues { get; set; }
        public Vertex SelectedVertex { get; set; }

        public PasteCommand(Vertex selVertex, List<GridViewCellClipboardEventArgs> pastedCells, List<Object> oldValues)
        {
            this.SelectedVertex = selVertex;
            this.PastedCells = pastedCells;
            this.PreviousValues = oldValues;
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

            foreach (var cellEditCommand in this.CellEditCommandsList)
            {
                isUndoSuccess = isUndoSuccess && cellEditCommand.Undo();
            }

            return isUndoSuccess;
        }
    }
}