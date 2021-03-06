﻿using System;
using Marv.Common;

namespace Marv.Input
{
    public class CellEditCommand : ICommand
    {
        public string ColumnName { get; set; }
        public object NewData { get; set; }
        public object OldData { get; set; }
        public EvidenceRow Row { get; set; }
        public Vertex SelectedVertex { get; set; }

        public CellEditCommand(EvidenceRow row, string colName, Vertex selVertex, object newData, object oldData)
        {
            this.Row = row;
            this.ColumnName = colName;
            this.SelectedVertex = selVertex;
            this.NewData = newData;
            this.OldData = oldData;
        }

        public void Execute()
        {
            DateTime dateTime;

            if (ColumnName.TryParse(out dateTime))
            {
                Row[ColumnName] = this.SelectedVertex.States.ParseEvidenceString(NewData as string);
            }

            else if (ColumnName == "Comment")
            {
                Row[ColumnName] = NewData as string;
            }
            else
            {
                double val;
                Row[ColumnName] = Double.TryParse(NewData.ToString(), out val) ? Convert.ToDouble(NewData.ToString()) : 0;
            }
            this.SelectedVertex.IsUserEvidenceComplete = true;
        }

        public bool Undo()
        {
            if (Row == null)
            {
                return false;
            }

            DateTime dateTime;
            if (ColumnName.TryParse(out dateTime))
            {
                if (!OldData.Equals(""))
                {
                    Row[ColumnName] = OldData;
                }
                else
                {
                    Row[ColumnName] = "";
                }
                return true;
            }

            if (OldData == null)
            {
                OldData = "";
            }

            Row[ColumnName] = OldData;

            return true;
        }
    }
}