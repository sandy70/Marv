using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Marv.Common;

namespace Marv
{
    public class LineData : NotifyPropertyChanged, ILineData
    {
        public const string FileDescription = "MARV Pipeline Data";
        public const string FileExtension = "marv-linedata";

        private const int DefaultYear = 2010;

        private int endYear = DefaultYear;
        private Guid guid;
        private Dict<string, int, string, double[]> sectionBeliefs = new Dict<string, int, string, double[]>();
        private Dict<string, int, string, VertexEvidence> sectionEvidences = new Dict<string, int, string, VertexEvidence>();
        private int startYear = DefaultYear;

        public int EndYear
        {
            get { return this.endYear; }

            set
            {
                this.endYear = value;
                this.RaisePropertyChanged();

                if (this.EndYear < this.StartYear)
                {
                    this.StartYear = this.EndYear;
                }
            }
        }

        public Guid Guid
        {
            get { return this.guid; }

            set
            {
                if (value != this.guid)
                {
                    this.guid = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public Dict<string, int, string, double[]> SectionBeliefs
        {
            get { return this.sectionBeliefs; }

            set
            {
                if (value.Equals(this.sectionBeliefs))
                {
                    return;
                }

                this.sectionBeliefs = value;
                this.RaisePropertyChanged();
            }
        }

        public Dict<string, int, string, VertexEvidence> SectionEvidences
        {
            get { return this.sectionEvidences; }

            set
            {
                if (value != this.sectionEvidences)
                {
                    this.sectionEvidences = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<string> SectionIds
        {
            get { return this.sectionEvidences.Keys; }
        }

        public int StartYear
        {
            get { return this.startYear; }

            set
            {
                this.startYear = value;
                this.RaisePropertyChanged();

                if (this.StartYear > this.EndYear)
                {
                    this.EndYear = this.StartYear;
                }
            }
        }

        public LineData()
        {
            this.SectionEvidences.CollectionChanged += this.SectionEvidences_CollectionChanged;
            this.SectionEvidences.Keys.CollectionChanged += this.Keys_CollectionChanged;
        }

        public static ILineData Read(string filePath)
        {
            return Utils.ReadJson<LineData>(filePath);
        }

        public void ChangeSectionId(string oldId, string newId)
        {
            this.SectionBeliefs.ChangeKey(oldId, newId);
            this.SectionEvidences.ChangeKey(oldId, newId);
        }

        public Dict<int, string, double[]> GetBelief(string sectionId)
        {
            return this.SectionBeliefs[sectionId];
        }

        public double[,] GetBeliefStatistic(NetworkNode node, IVertexValueComputer valueComputer)
        {
            foreach (var sectionId in (IEnumerable<string>) this.sectionEvidences.Keys)
            {
                Console.WriteLine(sectionId);
            }

            return null;
        }

        public double[,] GetBeliefStatistic(NetworkNode node, IVertexValueComputer valueComputer, IEnumerable<string> sectionIds)
        {
            foreach (var sectionId in (IEnumerable<string>) this.sectionEvidences.Keys)
            {
                Console.WriteLine(sectionId);
            }

            return null;
        }

        public Dict<int, string, VertexEvidence> GetEvidence(string sectionId)
        {
            return this.sectionEvidences[sectionId];
        }

        public Task<Dict<int, string, VertexEvidence>> GetEvidenceAsync(string sectionId)
        {
            return Task.Run(() => this.sectionEvidences[sectionId]);
        }

        public void RemoveSection(string sectionId)
        {
            this.SectionBeliefs[sectionId] = null;
            this.SectionEvidences[sectionId] = null;
        }

        public void SetBelief(string sectionId, Dict<int, string, double[]> sectionBelief)
        {
            this.SectionBeliefs[sectionId] = sectionBelief;
        }

        public void SetEvidence(string sectionId, Dict<int, string, VertexEvidence> sectionEvidence)
        {
            this.SectionEvidences[sectionId] = sectionEvidence;
        }

        public void Write(string filePath)
        {
            this.WriteJson(filePath);
        }

        private void Keys_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    this.SectionBeliefs[item as string] = null;
                }
            }

            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    this.SectionBeliefs[item as string] = new Dict<int, string, double[]>();
                }
            }
        }

        private void SectionEvidences_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    var kvp = item as Kvp<string, Dict<int, string, VertexEvidence>>;
                    var yearEvidences = kvp.Value;

                    for (var year = this.StartYear; year <= this.EndYear; year++)
                    {
                        if (!yearEvidences.ContainsKey(year))
                        {
                            yearEvidences[year] = new Dict<string, VertexEvidence>();
                        }
                    }
                }
            }
        }
    }
}