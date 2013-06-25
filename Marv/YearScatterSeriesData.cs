namespace Marv
{
    public class YearScatterSeriesData : MultiPoint
    {
        private int year;

        public int Year
        {
            get
            {
                return this.year;
            }

            set
            {
                if (value != this.year)
                {
                    this.year = value;
                    this.OnPropertyChanged("Year");
                }
            }
        }
    }
}