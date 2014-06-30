using Marv.Common;
using Marv.Common.Map;

namespace Marv.Controls.Map
{
    public class LocationEllipse : Model
    {
        private Location center;
        private double radius;

        public Location Center
        {
            get
            {
                return this.center;
            }

            set
            {
                if (value != this.center)
                {
                    this.center = value;
                    this.RaisePropertyChanged("Center");
                }
            }
        }

        public double Radius
        {
            get
            {
                return this.radius;
            }

            set
            {
                this.radius = value;
                this.RaisePropertyChanged("Radius");
            }
        }
    }
}