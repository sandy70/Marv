namespace Marv.Common
{
    public interface IEvidence
    {
        string EvidenceString { get; set; }

        void Set(Graph bnGraph, string vertexKey);
    }
}