namespace Marv
{
    public class CategoricalPoint : NotifyPropertyChanged, IKeyed
    {
        private double? _value;
        private object category;

        public object Category
        {
            get
            {
                return this.category;
            }

            set
            {
                if (value != this.category)
                {
                    this.category = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public double? Value
        {
            get
            {
                return this._value;
            }

            set
            {
                if (value != this._value)
                {
                    this._value = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        string IKeyed<string>.Key
        {
            get
            {
                return this.Category as string;
            }
        }
    }
}