using System;
using System.Collections.Generic;
using Marv.Common;
using Telerik.Windows.Controls;

namespace Marv.Input
{
    internal class PasteCommand : ICommand
    {
        public List<AddRowCommand> AddRowCommandsList = new List<AddRowCommand>();
        public List<CellEditCommand> CellEditCommandsList = new List<CellEditCommand>();
        public List<GridViewCellClipboardEventArgs> PastedCells { get; set; }
        public List<Object> PreviousValues { get; set; }

        public Vertex SelectedVertex { get; set; }

        public PasteCommand(Vertex selVertex, List<GridViewCellClipboardEventArgs> pastedCells, List<Object> oldValues, List<AddRowCommand> addRowCommands)
        {
            this.SelectedVertex = selVertex;
            this.PastedCells = pastedCells;
            this.PreviousValues = oldValues;
            AddRowCommandsList = addRowCommands;
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
                foreach (var addrowCommand in AddRowCommandsList)
                {
                    if (addrowCommand.NewRow.Equals(cellEditCommand.Row))
                    {
                        addrowCommand.Undo();
                        break;
                    }
                }

                isUndoSuccess = isUndoSuccess && cellEditCommand.Undo();
            }

            return true;
        }
    }
}