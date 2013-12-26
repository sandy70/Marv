﻿using Marv.Common;

namespace Marv.Controls
{
    public class LocationEllipse : ViewModel
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
                if (value != this.radius)
                {
                    this.radius = value;
                    this.RaisePropertyChanged("Radius");
                }
            }
        }
    }
}