namespace Marv.Common.Graph
{
    public interface IEvidence
    {
        string SynergiString { get; set; }

        void Set(Graph graph, string vertexKey);
    }
}