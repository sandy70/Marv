using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Marv.Input
{
    public class EvidenceTable : ObservableCollection<EvidenceRow>
    {
        private readonly IEnumerable<DateTime> dateTimes;

        public EvidenceTable(IEnumerable<DateTime> theDateTimes)
        {
            this.dateTimes = theDateTimes;
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
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