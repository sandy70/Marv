using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Marv.Common.Types;

namespace Marv.Common
{
    public class LineDataFolder : NotifyPropertyChanged, ILineData
    {
        private const string BeliefsDirName = "SectionBeliefs";
        private const string EvidencesDirName = "SectionEvidences";
        private int endYear;
        private Guid guid;
        private string rootDirPathPath;
        private int startYear;

        public int EndYear
        {
            get { return this.endYear; }

            set
            {
                if (value.Equals(this.endYear))
                {
                    return;
                }

                this.endYear = value;
                this.RaisePropertyChanged();

                this.WriteHeader();
            }
        }

        public Guid Guid
        {
            get { return this.guid; }

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

        public ObservableCollection<string> SectionIds
        {
            get { return new ObservableCollection<string>(this.GetSectionIds()); }
        }

        public int StartYear
        {
            get { return this.startYear; }

            set
            {
                if (value.Equals(this.startYear))
                {
                    return;
                }

                this.startYear = value;
                this.RaisePropertyChanged();

                this.WriteHeader();
            }
        }

        public LineDataFolder() {}

        public LineDataFolder(string theRootDirPath)
        {
            this.rootDirPathPath = File.Exists(theRootDirPath) ? Path.GetDirectoryName(theRootDirPath) : theRootDirPath;

            if (!Directory.Exists(this.rootDirPathPath))
            {
                Directory.CreateDirectory(this.rootDirPathPath);
                Directory.CreateDirectory(Path.Combine(this.rootDirPathPath, BeliefsDirName));
                Directory.CreateDirectory(Path.Combine(this.rootDirPathPath, EvidencesDirName));
            }
        }

        public static ILineData Read(string filePath)
        {
            // This should read End, StartYears and Guid
            var lineData = Utils.ReadJson<LineDataFolder>(filePath);
            lineData.rootDirPathPath = Path.GetDirectoryName(filePath);

            return lineData;
        }

        public void ChangeSectionId(string oldId, string newId)
        {
            this.SectionIds.Replace(oldId, newId);

            var oldDirName = Path.Combine(this.rootDirPathPath, BeliefsDirName, oldId + ".marv-sectionbelief");
            var newDirName = Path.Combine(this.rootDirPathPath, BeliefsDirName, newId + ".marv-sectionbelief");

            File.Move(oldDirName, newDirName);

            oldDirName = Path.Combine(this.rootDirPathPath, EvidencesDirName, oldId + ".marv-sectionevidence");
            newDirName = Path.Combine(this.rootDirPathPath, EvidencesDirName, newId + ".marv-sectionevidence");

            File.Move(oldDirName, newDirName);
        }

        public Dict<int, string, double[]> GetBelief(string sectionId)
        {
            Console.WriteLine("Getting section [{0}] belief", sectionId);
            return Utils.ReadJson<Dict<int, string, double[]>>(Path.Combine(this.rootDirPathPath, BeliefsDirName, sectionId + ".marv-sectionbelief"));
        }

        public double[,] GetBeliefStatistic(IVertexValueComputer valueComputer, NetworkVertex networkVertex)
        {
            return this.GetBeliefStatistic(valueComputer, networkVertex, this.GetSectionIds());
        }

        public double[,] GetBeliefStatistic(IVertexValueComputer valueComputer, NetworkVertex networkVertex, IEnumerable<string> sectionIds)
        {
            var sectionIdList = sectionIds as IList<string> ?? sectionIds.ToList();

            var nRows = sectionIdList.Count();
            var nCols = this.GetBelief(sectionIdList.First()).Keys.Count;

            var statistic = new double[nRows, nCols];

            var row = 0;
            foreach (var sectionId in sectionIdList)
            {
                var col = 0;

                var sectionBelief = this.GetBelief(sectionId);

                if (sectionBelief == null)
                {
                    continue;
                }

                foreach (var year in sectionBelief.Keys)
                {
                    if (sectionBelief[year][networkVertex.Key] != null)
                    {
                        statistic[row, col] = valueComputer.Compute(networkVertex, sectionBelief[year][networkVertex.Key], null);
                    }

                    col++;
                }

                row++;
            }

            return statistic;
        }

        public double[,] GetBeliefs(NetworkVertex networkVertex, string sectionId, IEnumerable<int> years)
        {
            var sectionBelief = this.GetBelief(sectionId);

            var nRows = networkVertex.States.Count;
            var nCols = years.Count();

            var beliefs = new double[nRows, nCols];

            for (var col = 0; col < nCols; col++)
            {
                for (var row = 0; row < nRows; row++)
                {
                    beliefs[row, col] = sectionBelief[years.ElementAt(col)][networkVertex.Key][row];
                }
            }

            return beliefs;
        }

        public double[,] GetBeliefs(NetworkVertex networkVertex, IEnumerable<string> sectionIds, int year)
        {
            var sectionIdList = sectionIds as IList<string> ?? sectionIds.ToList();

            var nRows = sectionIdList.Count();
            var nCols = networkVertex.States.Count;

            var beliefs = new double[nRows, nCols];

            for (var row = 0; row < nRows; row++)
            {
                var sectionBelief = this.GetBelief(sectionIdList[row]);

                for (var col = 0; col < nCols; col++)
                {
                    beliefs[row, col] = sectionBelief[year][networkVertex.Key][col];
                }
            }

            return beliefs;
        }

        public Dict<int, string, VertexEvidence> GetEvidence(string sectionId)
        {
            Console.WriteLine("Getting evidence for section [{0}]", sectionId);
            return Utils.ReadJson<Dict<int, string, VertexEvidence>>(Path.Combine(this.rootDirPathPath, EvidencesDirName, sectionId + ".marv-sectionevidence"));
        }

        public Task<Dict<int, string, VertexEvidence>> GetEvidenceAsync(string sectionId)
        {
            Console.WriteLine("Getting evidence for section [{0}]", sectionId);
            return Task.Run(() => Utils.ReadJson<Dict<int, string, VertexEvidence>>(Path.Combine(this.rootDirPathPath, EvidencesDirName, sectionId + ".marv-sectionevidence")));
        }

        public double[,] GetEvidenceStatistic(NetworkVertex networkVertex, IVertexValueComputer valueComputer)
        {
            return this.GetEvidenceStatistic(networkVertex, valueComputer, this.GetSectionIds());
        }

        public double[,] GetEvidenceStatistic(NetworkVertex networkVertex, IVertexValueComputer valueComputer, IEnumerable<string> sectionIds)
        {
            var sectionIdList = sectionIds as IList<string> ?? sectionIds.ToList();

            var nRows = sectionIdList.Count();
            var nCols = this.GetEvidence(sectionIdList.First()).Keys.Count;

            var statistic = new double[nRows, nCols];

            var row = 0;
            foreach (var sectionId in sectionIdList)
            {
                var col = 0;

                var sectionEvidence = this.GetEvidence(sectionId);

                if (sectionEvidence == null)
                {
                    continue;
                }

                foreach (var year in sectionEvidence.Keys)
                {
                    if (sectionEvidence[year][networkVertex.Key].Value != null)
                    {
                        statistic[row, col] = valueComputer.Compute(networkVertex, sectionEvidence[year][networkVertex.Key].Value, null);
                    }

                    col++;
                }

                row++;
            }

            return statistic;
        }

        public IEnumerable<string> GetSectionIds()
        {
            var evidencesDir = Path.Combine(this.rootDirPathPath, EvidencesDirName);
            var list = Directory.EnumerateFiles(evidencesDir, "*.marv-sectionevidence").Select(Path.GetFileNameWithoutExtension).ToList();
            list.Sort(new AlphaNumericComparer());
            return list;
        }

        public void RemoveSection(string sectionId)
        {
            this.SectionIds.Remove(sectionId);

            var beliefFilePath = Path.Combine(this.rootDirPathPath, BeliefsDirName, sectionId + ".marv-sectionbelief");
            var evidenceFilePath = Path.Combine(this.rootDirPathPath, EvidencesDirName, sectionId + ".marv-sectionevidence");

            File.Delete(beliefFilePath);
            File.Delete(evidenceFilePath);
        }

        public void SetBelief(string sectionId, Dict<int, string, double[]> sectionBelief)
        {
            this.SectionIds.AddUnique(sectionId);

            var beliefFilePath = Path.Combine(this.rootDirPathPath, BeliefsDirName, sectionId + ".marv-sectionbelief");
            sectionBelief.WriteJson(beliefFilePath);
        }

        public void SetEvidence(string sectionId, Dict<int, string, VertexEvidence> sectionEvidence)
        {
            this.SectionIds.AddUnique(sectionId);

            var evidenceFilePath = Path.Combine(this.rootDirPathPath, EvidencesDirName, sectionId + ".marv-sectionevidence");
            sectionEvidence.WriteJson(evidenceFilePath);
        }

        public void Write(string filePath)
        {
            // Do nothing because files should be updated already.
        }

        public void WriteHeader()
        {
            if (this.rootDirPathPath != null)
            {
                this.WriteJson(Path.Combine(this.rootDirPathPath, Path.GetFileName(this.rootDirPathPath) + "." + LineData.FileExtension));
            }
        }
    }
}