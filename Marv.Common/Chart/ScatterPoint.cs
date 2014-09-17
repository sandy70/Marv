namespace Marv
{
    public class ScatterPoint : NotifyPropertyChanged
    {
        private double xValue;

        private double? yValue;

        public double XValue
        {
            get
            {
                return this.xValue;
            }

            set
            {
                if (value != this.xValue)
                {
                    this.xValue = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public double? YValue
        {
            get
            {
                return this.yValue;
            }

            set
            {
                if (value != this.yValue)
                {
                    this.yValue = value;
                    this.RaisePropertyChanged();
                }
            }
        }
    }
}