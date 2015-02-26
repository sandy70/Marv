using Marv.Common;
using Marv.Common.Types;

namespace Marv.Input
{
    public class RowAddedEventArgs
    {
        public Dict<int, string, NodeEvidence> SectionEvidence;
        public string SectionId;
    }
}