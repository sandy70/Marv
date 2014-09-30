using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Marv.Input
{
    public class LineData : NotifyPropertyChanged
    {
        public const int DefaultYear = 2010;
        public const string FileDescription = "MARV Pipeline Data";
        public const string FileExtension = "marv-linedata";

        private int endYear = DefaultYear;
        private Guid guid;
        private Dict<string, int, string, double[]> sectionBeliefs = new Dict<string, int, string, double[]>();
        private Dict<string, int, string, VertexEvidence> sectionEvidences = new Dict<string, int, string, VertexEvidence>();
        private int startYear = DefaultYear;

        public int EndYear
        {
            get
            {
                return this.endYear;
            }

            set
            {
                var oldEndYear = this.EndYear;

                this.endYear = value;
                this.RaisePropertyChanged();

                this.UpdateSections(this.StartYear, this.EndYear, this.StartYear, oldEndYear);

                if (this.EndYear < this.StartYear)
                {
                    this.StartYear = this.EndYear;
                }
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
                if (value != this.guid)
                {
                    this.guid = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public Dict<string, int, string, double[]> SectionBeliefs
        {
            get
            {
                return this.sectionBeliefs;
            }

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
            get
            {
                return this.sectionEvidences;
            }

            set
            {
                if (value != this.sectionEvidences)
                {
                    this.sectionEvidences = value;
                    this.RaisePropertyChanged();
                }
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
                var oldStartYear = this.StartYear;

                this.startYear = value;
                this.RaisePropertyChanged();

                this.UpdateSections(this.StartYear, this.EndYear, oldStartYear, this.endYear);

                if (this.StartYear > this.EndYear)
                {
                    this.EndYear = this.StartYear;
                }
            }
        }

        public LineData()
        {
            this.SectionEvidences.CollectionChanged += this.SectionEvidences_CollectionChanged;
            this.SectionEvidences.Keys.CollectionChanged += Keys_CollectionChanged;
        }

        public void AddSections(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                if (!this.SectionEvidences.ContainsKey(key))
                {
                    this.SectionEvidences[key] = new Dict<int, string, VertexEvidence>();
                }
            }

            this.RaiseDataChanged();
        }

        public void RaiseDataChanged()
        {
            if (this.DataChanged != null)
            {
                this.DataChanged(this, new EventArgs());
            }
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

        private void UpdateSections(int newStartYear, int newEndYear, int oldStartYear, int oldEndYear)
        {
            foreach (var sectionId in this.SectionEvidences.Keys.ToList())
            {
                var startYear = Utils.Min(newStartYear, oldStartYear);
                var endYear = Utils.Max(newEndYear, oldEndYear);

                for (var year = startYear; year <= endYear; year++)
                {
                    if (year < newStartYear || newEndYear < year)
                    {
                        this.SectionEvidences[sectionId][year] = null;
                    }
                    else
                    {
                        if (!this.SectionEvidences[sectionId].ContainsKey(year))
                        {
                            this.SectionEvidences[sectionId][year] = new Dict<string, VertexEvidence>();
                        }
                    }
                }
            }

            this.RaiseDataChanged();
        }

        public event EventHandler DataChanged;
    }
}