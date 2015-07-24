using Marv.Common;

namespace Marv.Input
{
    public class AddRowCommand : ICommand
    {
        public EvidenceRow NewRow { get; set; }
        public EvidenceTable Table { get; set; }

        public AddRowCommand(EvidenceRow row, EvidenceTable table)
        {
            this.NewRow = row;
            this.Table = table;
        }

        public void Execute() {}

        public bool Undo()
        {
            if (this.Table.Contains(NewRow))
            {
                this.Table.Remove(NewRow);
            }

            return true;
        }
    }
}