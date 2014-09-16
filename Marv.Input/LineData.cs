using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Marv;

namespace Marv.Input
{
    public class LineData : NotifyPropertyChanged
    {
        public static int DefaultYear = 2010;

        private int endYear = 2010;
        private Guid guid;
        private Dict<string, int, string, VertexData> sections = new Dict<string, int, string, VertexData>();
        private int startYear = 2010;

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

        public Dict<string, int, string, VertexData> Sections
        {
            get
            {
                return this.sections;
            }

            set
            {
                if (value != this.sections)
                {
                    this.sections = value;
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
            this.Sections.CollectionChanged += Sections_CollectionChanged;
        }

        private void Sections_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    var kvp = item as Kvp<string, Dict<int, string, VertexData>>;
                    var yearData = kvp.Value;

                    for (var year = this.StartYear; year <= this.EndYear; year++)
                    {
                        if (!yearData.ContainsKey(year))
                        {
                            yearData[year] = new Dict<string, VertexData>();
                        }
                    }
                }
            }
        }

        private void UpdateSections(int newStartYear, int newEndYear, int oldStartYear, int oldEndYear)
        {
            foreach (var sectionId in this.Sections.Keys)
            {
                var startYear = Utils.Min(newStartYear, oldStartYear);
                var endYear = Utils.Max(newEndYear, oldEndYear);

                for (var year = startYear; year <= endYear; year++)
                {
                    if (year < newStartYear || newEndYear < year)
                    {
                        this.Sections[sectionId][year] = null;
                    }
                    else
                    {
                        if (!this.Sections[sectionId].ContainsKey(year))
                        {
                            this.Sections[sectionId][year] = new Dict<string, VertexData>();
                        }
                    }
                }
            }

            this.RaiseDataChanged();
        }

        public void AddSections(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                if (!this.Sections.ContainsKey(key))
                {
                    this.Sections[key] = new Dict<int, string, VertexData>();
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

        public event EventHandler DataChanged;
    }
}