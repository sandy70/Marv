using Marv.Common;

namespace Marv.Controls
{
    public class LocationEllipse : NotifyPropertyChanged
    {
        private Location center;
        private double radius;

        public Location Center
        {
            get { return this.center; }

            set
            {
                if (value != this.center)
                {
                    this.center = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public double Radius
        {
            get { return this.radius; }

            set
            {
                this.radius = value;
                this.RaisePropertyChanged();
            }
        }
    }
}