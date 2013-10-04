using Marv.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;

namespace LibPipeline
{
    public class MultiLocation : SelectableCollection<Location>
    {
        private MultiLocationValue _value = new MultiLocationValue();
        private bool isEnabled = true;
        private bool isSelected = false;
        private string name = "";
        private Brush stroke = new SolidColorBrush(Colors.LightBlue);

        public MultiLocation()
            : base()
        {
        }

        public MultiLocation(IEnumerable<Location> locations)
            : base(locations)
        {
        }

        public event ValueEventHandler<double> ValueChanged;

        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }

            set
            {
                if (value != this.isEnabled)
                {
                    this.isEnabled = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("IsEnabled"));

                    if (!this.IsEnabled)
                    {
                        this.IsSelected = false;
                    }
                }
            }
        }

        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }

            set
            {
                if (this.IsEnabled)
                {
                    this.isSelected = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("IsSelected"));
                }
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                if (value != this.name)
                {
                    this.name = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Name"));
                }
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
                    this.RaisePropertyChanged("Stroke");
                }
            }
        }

        public MultiLocationValue Value
        {
            get
            {
                return this._value;
            }

            set
            {
                if (value != this._value)
                {
                    this._value = value;
                    this.RaisePropertyChanged("Value");

                    foreach (var location in this)
                    {
                        location.Value = this.Value[location.Name];
                    }

                    if (this.ValueChanged != null)
                    {
                        this.ValueChanged(this, new ValueEventArgs<double> { Value = 0 });
                    }
                }
            }
        }
    }
}