using System;
using System.Collections.Specialized;
using System.Linq;
using Marv.Common;
using Marv.Common.Graph;

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
                if (value != this.endYear)
                {
                    this.UpdateSections(this.StartYear, value, this.StartYear, this.endYear);

                    this.endYear = value;
                    this.RaisePropertyChanged();

                    if (this.EndYear < this.StartYear)
                    {
                        this.StartYear = this.EndYear;
                    }
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
                if (value != this.startYear)
                {
                    this.UpdateSections(value, this.endYear, this.startYear, this.endYear);

                    this.startYear = value;
                    this.RaisePropertyChanged();

                    if (this.StartYear > this.EndYear)
                    {
                        this.EndYear = this.StartYear;
                    }
                }
            }
        }

        public LineData()
        {
            this.Sections.CollectionChanged += Sections_CollectionChanged;
        }

        private void Section_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateYears();
        }

        private void Sections_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    var sectionKvp = item as Kvp<string, Dict<int, string, VertexData>>;
                    sectionKvp.Value.CollectionChanged -= Section_CollectionChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    var sectionKvp = item as Kvp<string, Dict<int, string, VertexData>>;
                    sectionKvp.Value.CollectionChanged += Section_CollectionChanged;
                }
            }

            this.UpdateYears();
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

        private void UpdateYears()
        {
            foreach (var section in this.Sections.Values)
            {
                if (section.Keys.Count == 0)
                {
                    section[DefaultYear] = new Dict<string, VertexData>();
                }
            }

            this.EndYear = this.Sections.Values.Max(section => section.Keys.Max());
            this.StartYear = this.Sections.Values.Min(section => section.Keys.Min());
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