using System.Collections;
using System.Windows.Media;
using Marv.Common.Map;
using System.Collections.Generic;

namespace Marv.Controls
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
