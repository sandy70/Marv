using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Marv.Common
{
    public class LineStringCollection : NotifyPropertyChanged
    {
        private LocationRect bounds;
        private ObservableCollection<LocationCollection> lineStrings = new ObservableCollection<LocationCollection>();

        public LocationRect Bounds
        {
            get { return this.bounds; }

            set
            {
                this.bounds = value;
                this.RaisePropertyChanged();
            }
        }

        public ObservableCollection<LocationCollection> LineStrings
        {
            get { return this.lineStrings; }

            private set
            {
                this.LineStrings.CollectionChanged -= LineStrings_CollectionChanged;

                this.lineStrings = value;
                this.RaisePropertyChanged();

                this.LineStrings.CollectionChanged += LineStrings_CollectionChanged;
            }
        }

        public LineStringCollection()
        {
            this.LineStrings = new ObservableCollection<LocationCollection>();
        }

        private void LineStrings_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var locationCollection in e.NewItems.Cast<LocationCollection>())
                {
                    this.Bounds = LocationRect.Union(this.Bounds, locationCollection.Bounds);
                }
            }
        }
    }
}