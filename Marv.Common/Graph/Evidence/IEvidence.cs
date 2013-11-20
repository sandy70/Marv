namespace Marv.Common
{
    public interface IEvidence
    {
        string String { get; set; }

        string GetValue(Graph graph, string vertexKey);

        void Set(Graph graph, string vertexKey);
    }
}