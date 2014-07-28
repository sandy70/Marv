using System;
using System.Collections.Generic;
using System.Linq;
using Marv.Common;
using Newtonsoft.Json;

namespace Marv.Input
{
    public class LineEvidence
    {
        public readonly List<SectionEvidence, string> SectionEvidences = new List<SectionEvidence, string>();
        public Guid GraphGuid { get; set; }

        [JsonIgnore]
        public IEnumerable<int> Years
        {
            get
            {
                var years = new List<int>();

                foreach (var sectionEvidence in this.SectionEvidences)
                {
                    years.AddRange(sectionEvidence.YearEvidences.Select(yearEvidence => yearEvidence.Year));
                }

                years.Sort();

                return years.Distinct();
            }
        }
    }
}