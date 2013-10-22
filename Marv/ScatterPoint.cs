using Marv.Common;
using System.Collections.Generic;
using System.Linq;
using Telerik.Charting;

namespace Marv
{
    public class ScatterPoint : ViewModel
    {
        private double y;
        private int year;

        public double Y
        {
            get
            {
                return this.y;
            }

            set
            {
                if (value != this.y)
                {
                    this.y = value;
                    this.RaisePropertyChanged("Y");

                    var scatter = new ScatterDataPoint();
                }
            }
        }

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
                    this.RaisePropertyChanged("Year");
                }
            }
        }
    }
}