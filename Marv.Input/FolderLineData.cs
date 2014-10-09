using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Marv.Input
{
    public class FolderLineData : NotifyPropertyChanged, ILineData
    {
        private const string BeliefsDirName = "SectionBeliefs";
        private const string EvidencesDirName = "SectionEvidences";
        private int endYear;
        private Guid guid;
        private string rootDirPath;
        private int startYear;

        public int EndYear
        {
            get
            {
                return this.endYear;
            }

            set
            {
                if (value.Equals(this.endYear))
                {
                    return;
                }

                this.endYear = value;
                this.RaisePropertyChanged();
            }
        }

        public Guid Guid
        {
            get
            {
                return this.guid;
            }

            set
            {
                if (value.Equals(this.guid))
                {
                    return;
                }

                this.guid = value;
                this.RaisePropertyChanged();
            }
        }

        public int StartYear
        {
            get
            {
                return this.startYear;
            }

            set
            {
                if (value.Equals(this.startYear))
                {
                    return;
                }

                this.startYear = value;
                this.RaisePropertyChanged();
            }
        }

        private FolderLineData() {}

        public static ILineData Read(string filePath)
        {
            // This should read End, StartYears and Guid
            var lineData = Utils.ReadJson<FolderLineData>(filePath);
            lineData.rootDirPath = Path.GetDirectoryName(filePath);

            return lineData;
        }

        public void AddSections(IEnumerable<string> theSectionIds)
        {
            foreach (var sectionId in theSectionIds)
            {
                var sectionBelief = new Dict<int, string, double[]>();
                var sectionEvidence = new Dict<int, string, VertexEvidence>();

                for (int year = this.StartYear; year <= this.EndYear; year ++)
                {
                    sectionBelief[year] = new Dict<string, double[]>();
                    sectionEvidence[year] = new Dict<string, VertexEvidence>();
                }

                var beliefFilePath = Path.Combine(this.rootDirPath, "SectionBeliefs", sectionId +  ".marv-sectionbelief");
                var evidenceFilePath = Path.Combine(this.rootDirPath, "SectionEvidences", sectionId + ".marv-sectionevidence");

                sectionBelief.WriteJson(beliefFilePath);
                sectionEvidence.WriteJson(evidenceFilePath);
            }
        }

        public void ChangeSectionId(string oldId, string newId)
        {
            var oldDirName = Path.Combine(this.rootDirPath, BeliefsDirName, oldId + ".marv-sectionbelief");
            var newDirName = Path.Combine(this.rootDirPath, BeliefsDirName, newId + ".marv-sectionbelief");

            File.Move(oldDirName, newDirName);

            oldDirName = Path.Combine(this.rootDirPath, EvidencesDirName, oldId + ".marv-sectionevidence");
            newDirName = Path.Combine(this.rootDirPath, EvidencesDirName, newId + ".marv-sectionevidence");

            File.Move(oldDirName, newDirName);
        }

        public bool ContainsSection(string sectionId)
        {
            var beliefDir = Path.Combine(this.rootDirPath, BeliefsDirName, sectionId);
            var evidenceDir = Path.Combine(this.rootDirPath, EvidencesDirName, sectionId);

            return Directory.Exists(beliefDir) || Directory.Exists(evidenceDir);
        }

        public Dict<int, string, double[]> GetSectionBelief(string sectionId)
        {
            return Utils.ReadJson<Dict<int, string, double[]>>(Path.Combine(this.rootDirPath, BeliefsDirName, sectionId + ".marv-sectionbelief"));
        }

        public Dict<int, string, VertexEvidence> GetSectionEvidence(string sectionId)
        {
            Console.WriteLine("Reading section: " + sectionId);
            return Utils.ReadJson<Dict<int, string, VertexEvidence>>(Path.Combine(this.rootDirPath, EvidencesDirName, sectionId + ".marv-sectionevidence"));
        }

        public IEnumerable<string> GetSectionIds()
        {
            var evidencesDir = Path.Combine(this.rootDirPath, EvidencesDirName);
            return Directory.EnumerateFiles(evidencesDir, "*.marv-sectionevidence").Select(Path.GetFileNameWithoutExtension);
        }

        public void RaiseDataChanged()
        {
            if (this.DataChanged != null)
            {
                this.DataChanged(this, new EventArgs());
            }
        }

        public void RemoveSection(string sectionId)
        {
            var beliefFilePath = Path.Combine(this.rootDirPath, BeliefsDirName, sectionId + ".marv-sectionbelief");
            var evidenceFilePath = Path.Combine(this.rootDirPath, EvidencesDirName, sectionId + ".marv-sectionevidence");

            File.Delete(beliefFilePath);
            File.Delete(evidenceFilePath);
        }

        public void SetSectionBelief(string sectionId, Dict<int, string, double[]> sectionBelief)
        {
            var beliefFilePath = Path.Combine(this.rootDirPath, BeliefsDirName, sectionId + ".marv-sectionbelief");
            sectionBelief.WriteJson(beliefFilePath);
        }

        public void SetSectionEvidence(string sectionId, Dict<int, string, VertexEvidence> sectionEvidence)
        {
            var evidenceFilePath = Path.Combine(this.rootDirPath, EvidencesDirName, sectionId + ".marv-sectionevidence");
            sectionEvidence.WriteJson(evidenceFilePath);
        }

        public event EventHandler DataChanged;
    }
}