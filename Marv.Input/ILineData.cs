using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Marv.Input
{
    public interface ILineData
    {
        int EndYear { get; set; }
        Guid Guid { get; set; }
        int StartYear { get; set; }
        void AddSections(IEnumerable<string> keys);
        void ChangeSectionId(string oldId, string newId);
        bool ContainsSection(string sectionId);
        Dict<int, string, double[]> GetSectionBelief(string sectionId);
        Dict<int, string, VertexEvidence> GetSectionEvidence(string sectionId);
        ObservableCollection<string> GetSectionIds();
        void RaiseDataChanged();
        void RemoveSectionEvidence(string sectionId);
        void SetSectionBelief(string sectionId, Dict<int, string, double[]> sectionBelief);
        void SetSectionEvidence(string sectionId, Dict<int, string, VertexEvidence> sectionEvidence);
        event EventHandler DataChanged;
    }
}