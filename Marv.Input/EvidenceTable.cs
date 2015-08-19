using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Newtonsoft.Json;

namespace Marv.Input
{
    public class EvidenceTable : ObservableCollection<EvidenceRow>
    {

        private IEnumerable<DateTime> dateTimes;

        public IEnumerable<DateTime> DateTimes
        {
            get
            {
                if (this.dateTimes != null || this.Count <= 0)
                {
                    return this.dateTimes;
                }

                var theDateTimes = new List<DateTime>();

                var row = this.First();

                foreach (var dynamicMemeberName in row.GetDynamicMemberNames())
                {
                    DateTime dateTime;

                    if (dynamicMemeberName.TryParse(out dateTime))
                    {
                        theDateTimes.Add(dateTime);
                    }
                }

                return this.dateTimes = theDateTimes;
            }

            set { this.dateTimes = value; }
        }

        public EvidenceTable() {}

        public EvidenceTable(IEnumerable<DateTime> theDateTimes)
        {
            this.DateTimes = theDateTimes;
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && this.dateTimes != null)
            {
                foreach (var evidenceRow in e.NewItems.Cast<EvidenceRow>())
                {
                    foreach (var dateTime in this.dateTimes)
                    {
                        evidenceRow[dateTime.String()] = "";
                    }
                }
            }

            base.OnCollectionChanged(e);
        }
    }
}