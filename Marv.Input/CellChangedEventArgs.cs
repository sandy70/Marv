using Marv.Common;
using Marv.Common.Types;

namespace Marv.Input
{
    public class CellChangedEventArgs
    {
        public CellModel CellModel;
        public string NewString;
        public string OldString;
        public VertexEvidence VertexEvidence;
    }
}