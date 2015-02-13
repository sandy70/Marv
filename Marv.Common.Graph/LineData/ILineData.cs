using System;
using System.Collections.Generic;

namespace Marv
{
    public interface ILineData
    {
        int EndYear { get; set; }
        Guid Guid { get; set; }
        int StartYear { get; set; }
        void ReplaceSectionId(string oldId, string newId);
        Dict<int, string, double[]> GetBelief(string sectionId);
        Dict<int, string, VertexEvidence> GetEvidence(string sectionId);
        IEnumerable<string> GetSectionIds();
        void RemoveSection(string sectionId);
        void SetBelief(string sectionId, Dict<int, string, double[]> sectionBelief);
        void SetEvidence(string sectionId, Dict<int, string, VertexEvidence> sectionEvidence);
        void Write(string filePath);
        event EventHandler DataChanged;
    }
}