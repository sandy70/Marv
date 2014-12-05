using System.Collections.Generic;
using System.Windows.Media;
using Marv.Common;
using Marv.Map;

namespace Marv.Controls.Map
{
    public class LocationCollectionViewModel : NotifyPropertyChanged
    {
        private LocationCollection locations;
        private Brush stroke;

        public LocationCollection Locations
        {
            get
            {
                return this.locations;
            }

            set
            {
                if (value.Equals(this.locations))
                {
                    return;
                }

                this.locations = value;
                this.RaisePropertyChanged();
            }
        }

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
                    this.RaisePropertyChanged();
                }
            }
        }
    }
}