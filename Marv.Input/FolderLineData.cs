using System;
using System.Collections.Generic;
using System.IO;

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
                var beliefFolderName = Path.Combine(this.rootDirPath, "SectionBeliefs", sectionId);
                var evidenceFolderName = Path.Combine(this.rootDirPath, "SectionEvidences", sectionId);

                Directory.CreateDirectory(beliefFolderName);
                Directory.CreateDirectory(evidenceFolderName);
            }
        }

        public void ChangeSectionId(string oldId, string newId)
        {
            var oldDirName = Path.Combine(this.rootDirPath, BeliefsDirName, oldId);
            var newDirName = Path.Combine(this.rootDirPath, BeliefsDirName, newId);

            Directory.Move(oldDirName, newDirName);

            oldDirName = Path.Combine(this.rootDirPath, EvidencesDirName, oldId);
            newDirName = Path.Combine(this.rootDirPath, EvidencesDirName, newId);

            Directory.Move(oldDirName, newDirName);
        }

        public bool ContainsSection(string sectionId)
        {
            var beliefDir = Path.Combine(this.rootDirPath, BeliefsDirName, sectionId);
            var evidenceDir = Path.Combine(this.rootDirPath, EvidencesDirName, sectionId);

            return Directory.Exists(beliefDir) || Directory.Exists(evidenceDir);
        }

        public Dict<int, string, double[]> GetSectionBelief(string sectionId)
        {
            var beliefDir = Path.Combine(this.rootDirPath, BeliefsDirName, sectionId);
            var sectionBelief = new Dict<int, string, double[]>();

            foreach (var fileName in Directory.EnumerateFiles(beliefDir, "*." + Network.BeliefFileExtension))
            {
                var year = int.Parse(Path.GetFileNameWithoutExtension(fileName));
                sectionBelief[year] = Utils.ReadJson<Dict<string, double[]>>(fileName);
            }

            return sectionBelief;
        }

        public Dict<int, string, VertexEvidence> GetSectionEvidence(string sectionId)
        {
            var evidenceDir = Path.Combine(this.rootDirPath, EvidencesDirName, sectionId);
            var sectionEvidence = new Dict<int, string, VertexEvidence>();

            foreach (var fileName in Directory.EnumerateFiles(evidenceDir, "*." + Network.EvidenceFileExtension))
            {
                var year = int.Parse(Path.GetFileNameWithoutExtension(fileName));
                sectionEvidence[year] = Utils.ReadJson<Dict<string, VertexEvidence>>(fileName);
            }

            return sectionEvidence;
        }

        public IEnumerable<string> GetSectionIds()
        {
            var evidencesDir = Path.Combine(this.rootDirPath, EvidencesDirName);
            return Directory.EnumerateDirectories(evidencesDir);
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
            var beliefDir = Path.Combine(this.rootDirPath, BeliefsDirName, sectionId);
            var evidenceDir = Path.Combine(this.rootDirPath, EvidencesDirName, sectionId);

            Directory.Delete(beliefDir);
            Directory.Delete(evidenceDir);
        }

        public void SetSectionBelief(string sectionId, Dict<int, string, double[]> sectionBelief)
        {
            var beliefDir = Path.Combine(this.rootDirPath, BeliefsDirName, sectionId);

            foreach (var year in sectionBelief.Keys)
            {
                var beliefFileName = Path.Combine(beliefDir, year + "." + Network.BeliefFileExtension);
                sectionBelief[year].WriteJson(beliefFileName);
            }
        }

        public void SetSectionEvidence(string sectionId, Dict<int, string, VertexEvidence> sectionEvidence)
        {
            var evidenceDir = Path.Combine(this.rootDirPath, EvidencesDirName, sectionId);

            foreach (var year in sectionEvidence.Keys)
            {
                var evidenceFileName = Path.Combine(evidenceDir, year + "." + Network.EvidenceFileExtension);
                sectionEvidence[year].WriteJson(evidenceFileName);
            }
        }

        public event EventHandler DataChanged;
    }
}