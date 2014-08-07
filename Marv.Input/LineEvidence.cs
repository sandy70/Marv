using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Marv.Common;
using Newtonsoft.Json;

namespace Marv.Input
{
    public class LineEvidence
    {
        private readonly List<SectionEvidence, string> sectionEvidences = new List<SectionEvidence, string>();
        private List<int> years;
        private bool isYearsChanged = false;

        public Guid GraphGuid { get; set; }

        public List<SectionEvidence, string> SectionEvidences
        {
            get { return sectionEvidences; }
        }

        [JsonIgnore]
        public List<int> Years
        {
            get
            {
                if (isYearsChanged)
                {
                    UpdateYears();
                }
                
                return years;
            }
        }

        public LineEvidence()
        {
            this.SectionEvidences.CollectionChanged += SectionEvidences_CollectionChanged;
        }

        private void SectionEvidences_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            isYearsChanged = true;
        }

        private void UpdateYears()
        {
            this.years = new List<int>();

            foreach (var sectionEvidence in this.SectionEvidences)
            {
                years.AddUnique(sectionEvidence.YearEvidences.Select(yearEvidence => yearEvidence.Year));
            }

            this.years.Sort();
            isYearsChanged = false;
        }
    }
}