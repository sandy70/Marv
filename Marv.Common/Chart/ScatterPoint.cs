using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marv.Common
{
    public class ScatterPoint : ViewModel
    {
        private double xValue;

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
                    this.RaisePropertyChanged("XValue");
                }
            }
        }

        private double? yValue;

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
                    this.RaisePropertyChanged("YValue");
                }
            }
        }
    }
}
