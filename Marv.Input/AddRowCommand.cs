using Marv.Common;

namespace Marv.Input
{
    public class AddRowCommand : ICommand
    {
        public EvidenceTable Table { get; set; }

        public AddRowCommand(EvidenceTable table)
        {
            this.Table = table;
        }

        public void Execute() {}

        public bool Undo()
        {
            if (this.Table.Count == 0)
            {
                return false;
            }

            this.Table.RemoveAt(this.Table.Count - 1);
            return true;
        }
    }
}