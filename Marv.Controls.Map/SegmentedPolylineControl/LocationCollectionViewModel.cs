﻿using System.Collections.Generic;
using System.Windows.Media;
using Marv.Common.Map;

namespace Marv.Controls.Map
{
    public class LocationCollectionViewModel : LocationCollection
    {
        private Brush stroke;

        public Brush Stroke
        {
            get
            {
                return this.stroke;
            }

            set
            {
                if (value != this.stroke)
                {
                    this.stroke = value;
                    this.RaisePropertyChanged("Stroke");
                }
            }
        }

        public LocationCollectionViewModel(IEnumerable<Location> locations): base(locations)
        {
            
        }
    }
}