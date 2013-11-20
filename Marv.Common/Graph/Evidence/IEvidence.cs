namespace Marv.Common
{
    public interface IEvidence
    {
        string String { get; set; }

        void Set(Graph bnGraph, string vertexKey);
    }
}