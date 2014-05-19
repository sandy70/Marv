namespace Marv.Common.Graph
{
    public interface IEvidenceStringParser
    {
        Evidence Parse(Vertex vertex, string str);
    }
}