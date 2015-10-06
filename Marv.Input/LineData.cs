using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Marv.Common.Types;

namespace Marv.Input
{
    internal class LineData
    {
        private double baseTableMax = 100;
        private double baseTableMin;
        private double baseTableRange = 10;
        private DateTime endDate = DateTime.Now;
        private DateTime startDate = DateTime.Now;
        private Dict<string, NodeData> userDataObj = new Dict<string, NodeData>();

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

        public Dict<string, NodeData> UserDataObj
        {
            get { return userDataObj; }
            set { userDataObj = value; }
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