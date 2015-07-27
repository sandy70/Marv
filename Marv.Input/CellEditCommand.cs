using System;
using Marv.Common;
using Telerik.Windows.Controls;

namespace Marv.Input
{
    public class CellEditCommand : ICommand
    {
        public string ColumnName { get; set; }
        public GridViewCellEditEndedEventArgs E { get; set; }
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
            else
            {
                Row[ColumnName] = Convert.ToDouble(NewData.ToString());
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

            Row[ColumnName] = OldData;
            return true;
        }
    }
}