using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Marv.Common.Types;

namespace Marv.Common
{
    public interface ILineData
    {
        int EndYear { get; set; }
        Guid Guid { get; set; }
        ObservableCollection<string> SectionIds { get; }
        int StartYear { get; set; }

        void ChangeSectionId(string oldId, string newId);
        Dict<int, string, double[]> GetBelief(string sectionId);
        Dict<int, string, NodeEvidence> GetEvidence(string sectionId);
        Task<Dict<int, string, NodeEvidence>> GetEvidenceAsync(string sectionId);
        void RemoveSection(string sectionId);
        void SetBelief(string sectionId, Dict<int, string, double[]> sectionBelief);
        void SetEvidence(string sectionId, Dict<int, string, NodeEvidence> sectionEvidence);
        void Write(string filePath);
    }
}