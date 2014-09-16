using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marv
{
    public class ScatterPoint : Model
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
                    this.RaisePropertyChanged();
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
                    this.RaisePropertyChanged();
                }
            }
        }
    }
}
