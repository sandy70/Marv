﻿using Marv.Common;
using Newtonsoft.Json;

namespace Marv.Input
{
    public class SectionEvidence : IKey<string>
    {
        public readonly List<YearEvidence, int> YearEvidences = new List<YearEvidence, int>();
        public string Id;

        [JsonIgnore]
        public string Key
        {
            get
            {
                return this.Id;
            }

            set
            {
                this.Id = value;
            }
        }
    }
}