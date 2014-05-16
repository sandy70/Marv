namespace Marv.Common.Graph
{
    public class NullEvidenceString : IEvidenceStringParser
    {
        public Evidence Parse(Vertex vertex, string str)
        {
            return null;
        }
    }
}