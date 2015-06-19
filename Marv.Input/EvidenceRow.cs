using System.ComponentModel.DataAnnotations;
using Marv.Common.Types;

namespace Marv.Input
{
    public class EvidenceRow : Dynamic
    {
        [Display(AutoGenerateField = false)] public bool IsActive = true;
        [Display(AutoGenerateField = false)] public bool IsValid = true;
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

        public bool Contains(EvidenceRow other)
        {
            return this.From <= other.From && other.To <= this.To;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((EvidenceRow) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.@from.GetHashCode() * 397) ^ this.to.GetHashCode();
            }
        }

        private bool Equals(EvidenceRow other)
        {
            return this.@from.Equals(other.@from) && this.to.Equals(other.to);
        }
    }
}