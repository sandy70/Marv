using System.ComponentModel;

namespace LibPipeline
{
    public class Percentile : INotifyPropertyChanged
    {
        private double _Average;

        private double _DistanceFromOrigin;

        private double _Down90;

        private double _Up90;

        public Percentile()
        {
            this.Average = 0;
            this.DistanceFromOrigin = 0;
            this.Down90 = 0;
            this.Up90 = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public double Average
        {
            get { return _Average; }
            set
            {
                _Average = value;
                OnPropertyChanged("Average");
            }
        }

        public double DistanceFromOrigin
        {
            get { return _DistanceFromOrigin; }
            set
            {
                _DistanceFromOrigin = value;
                OnPropertyChanged("DistanceFromOrigin");
            }
        }

        public double Down90
        {
            get { return _Down90; }
            set
            {
                _Down90 = value;
                OnPropertyChanged("Down90");
            }
        }

        public double Up90
        {
            get { return _Up90; }
            set
            {
                _Up90 = value;
                OnPropertyChanged("Up90");
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}