using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Marv
{
    public interface ILineData
    {
        int EndYear { get; set; }
        Guid Guid { get; set; }
        ObservableCollection<string> SectionIds { get; }
        int StartYear { get; set; }

        void ChangeSectionId(string oldId, string newId);
        Dict<int, string, double[]> GetBelief(string sectionId);
        Dict<int, string, VertexEvidence> GetEvidence(string sectionId);
        Task<Dict<int, string, VertexEvidence>> GetEvidenceAsync(string sectionId);
        void RemoveSection(string sectionId);
        void SetBelief(string sectionId, Dict<int, string, double[]> sectionBelief);
        void SetEvidence(string sectionId, Dict<int, string, VertexEvidence> sectionEvidence);
        void Write(string filePath);
    }
}