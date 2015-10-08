using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Marv.Common.Types;
using Newtonsoft.Json;

namespace Marv.Input
{
    public class LineData
    {
        private double baseTableMax;
        private double baseTableMin;
        private double baseTableRange;
        private ObservableCollection<EvidenceRow> commentBlocks = new ObservableCollection<EvidenceRow>();
        private DateTime endDate;
        private DateTime startDate;
        private Dict<string, NodeData> userDataObj;

        [JsonProperty]
        public double BaseTableMax
        {
            get { return this.baseTableMax; }
            set
            {
                if (value.Equals(this.baseTableMax))
                {
                    return;
                }

                this.baseTableMax = value;
                this.RaisePropertyChanged();
            }
        }

        [JsonProperty]
        public double BaseTableMin
        {
            get { return this.baseTableMin; }
            set
            {
                if (value.Equals(this.baseTableMin))
                {
                    return;
                }

                this.baseTableMin = value;
                this.RaisePropertyChanged();
            }
        }

        [JsonProperty]
        public double BaseTableRange
        {
            get { return this.baseTableRange; }
            set
            {
                if (value.Equals(this.baseTableRange))
                {
                    return;
                }

                this.baseTableRange = value;
                this.RaisePropertyChanged();
            }
        }

        [JsonProperty]
        public ObservableCollection<EvidenceRow> CommentBlocks
        {
            get { return commentBlocks; }
            set
            {
                this.commentBlocks = value;
                this.RaisePropertyChanged();
            }
        }


        public DateTime EndDate
        {
            get { return this.endDate; }

            set
            {
                if (value.Equals(this.endDate))
                {
                    return;
                }

                this.endDate = value;
                this.RaisePropertyChanged();
            }
        }

        public DateTime StartDate
        {
            get { return this.startDate; }

            set
            {
                if (value.Equals(this.startDate))
                {
                    return;
                }

                this.startDate = value;
                this.RaisePropertyChanged();
            }
        }

        [JsonProperty]
        public Dict<string, NodeData> UserDataObj
        {
            get { return this.userDataObj; }
            set
            {
                userDataObj = value;
                this.RaisePropertyChanged();
            }
        }

        public LineData()
        {
            BaseTableMin = 0;
            BaseTableMax = 100;
            BaseTableRange = 10;
            StartDate = DateTime.Now;
            EndDate = DateTime.Now;
            UserDataObj = new Dict<string, NodeData>();
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null && propertyName != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}