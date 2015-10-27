using System.ComponentModel.DataAnnotations;
using Marv.Common.Types;
using Newtonsoft.Json;

namespace Marv.Input
{
    public class EvidenceRow : Dynamic
    {
        private string comment = "";
        private double from;
        private double to;

        [Display(Order = 2)]
        [JsonProperty]
        public string Comment
        {
            get { return comment; }
            set
            {
                comment = value;
                this.RaisePropertyChanged();
            }
        }

        [Display(Order = 0)]
        [JsonProperty]
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
        [JsonProperty]
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

        public bool Contains(EvidenceRow other)
        {
            if (!other.From.Equals(other.To))
            {
                return this.From <= other.From && other.To <= this.To;
            }

            // Ex: section such as 0-12/12-24 should not be picked up for point value 12-12
            var pointValue = other.From;

            if (this.From.Equals(pointValue) || this.To.Equals(pointValue))
            {
                return this.From.Equals(other.From) && this.To.Equals(other.To);
            }

            return this.From <= other.From && other.To <= this.To;
        }
    }
}