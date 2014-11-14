namespace Marv
{
    public class CategoricalPoint : NotifyPropertyChanged, IKeyed
    {
        private object category;
        private int name;
        private double? value;

        public object Category
        {
            get { return this.category; }

            set
            {
                if (value != this.category)
                {
                    this.category = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        string IKeyed<string>.Key
        {
            get { return this.Category as string; }
        }

        public int Name
        {
            get { return this.name; }

            set
            {
                if (value.Equals(this.name))
                {
                    return;
                }

                this.name = value;
                this.RaisePropertyChanged();
            }
        }

        public double? Value
        {
            get { return this.value; }

            set
            {
                if (value != this.value)
                {
                    this.value = value;
                    this.RaisePropertyChanged();
                }
            }
        }
    }
}