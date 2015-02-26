using Marv.Common;

namespace Marv.Input
{
    public class CellChangedEventArgs
    {
        public CellModel CellModel;
        public string NewString;
        public string OldString;
        public NodeEvidence NodeEvidence;
    }
}