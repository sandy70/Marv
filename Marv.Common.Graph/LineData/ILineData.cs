using System;
using System.Collections.Generic;

namespace Marv
{
    public interface ILineData
    {
        int EndYear { get; set; }
        Guid Guid { get; set; }
        int StartYear { get; set; }
        void AddSections(IEnumerable<string> theSectionIds);
        void ReplaceSectionId(string oldId, string newId);
        bool ContainsSection(string sectionId);
        Dict<int, string, double[]> GetSectionBelief(string sectionId);
        Dict<int, string, VertexEvidence> GetSectionEvidence(string sectionId);
        IEnumerable<string> GetSectionIds();
        void RaiseDataChanged();
        void RemoveSection(string sectionId);
        void SetSectionBelief(string sectionId, Dict<int, string, double[]> sectionBelief);
        void SetSectionEvidence(string sectionId, Dict<int, string, VertexEvidence> sectionEvidence);
        void Write(string filePath);
        event EventHandler DataChanged;

        void AddSection(string sectionId);
    }
}