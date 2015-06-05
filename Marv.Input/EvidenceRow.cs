using System.ComponentModel.DataAnnotations;
using Marv.Common.Types;

namespace Marv.Input
{
    public class EvidenceRow : Dynamic
    {
        private double from;
        private double to;

        [Display(Order = 0)]
        public double From
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

        [Display(Order = 1)]
        public double To
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