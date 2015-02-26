using Marv.Common;
using Marv.Common.Types;

namespace Marv.Input
{
    public class RowAddedEventArgs
    {
        public Dict<int, string, VertexEvidence> SectionEvidence;
        public string SectionId;
    }
}