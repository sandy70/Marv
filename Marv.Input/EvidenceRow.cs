using Marv.Common.Types;

namespace Marv.Input
{
    public class EvidenceRow : Dynamic
    {
        private double? from;
        private double? to;

        public double? From
        {
            get { return this.from; }

            set
            {
                if (value.Equals(this.from))
                {
                    return;
                }

                this.from = value;
                this.RaisePropertyChanged();
            }
        }

        public double? To
        {
            get { return this.to; }

            set
            {
                if (value.Equals(this.to))
                {
                    return;
                }

                this.to = value;
                this.RaisePropertyChanged();
            }
        }
    }
}