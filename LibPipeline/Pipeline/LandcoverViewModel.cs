using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace LibPipeline
{
    public class LandcoverViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Point> _BarrenPoints;
        private ObservableCollection<Point> _DevelopedPoints;
        private ObservableCollection<Point> _FarmPoints;
        private ObservableCollection<Point> _ForestPoints;
        private ObservableCollection<Point> _GrassPoints;
        private ObservableCollection<Point> _ShrubsPoints;
        private ObservableCollection<Point> _UnknownPoints;
        private ObservableCollection<Point> _WaterIcePoints;
        private ObservableCollection<Point> _WetlandPoints;

        public LandcoverViewModel()
        {
            this.BarrenPoints = new ObservableCollection<Point>();
            this.DevelopedPoints = new ObservableCollection<Point>();
            this.FarmPoints = new ObservableCollection<Point>();
            this.ForestPoints = new ObservableCollection<Point>();
            this.GrassPoints = new ObservableCollection<Point>();
            this.ShrubsPoints = new ObservableCollection<Point>();
            this.UnknownPoints = new ObservableCollection<Point>();
            this.WaterIcePoints = new ObservableCollection<Point>();
            this.WetlandPoints = new ObservableCollection<Point>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Point> BarrenPoints
        {
            get { return _BarrenPoints; }
            set
            {
                _BarrenPoints = value;
                OnPropertyChanged("BarrenPoints");
            }
        }

        public ObservableCollection<Point> DevelopedPoints
        {
            get { return _DevelopedPoints; }
            set
            {
                _DevelopedPoints = value;
                OnPropertyChanged("DevelopedPoints");
            }
        }

        public ObservableCollection<Point> FarmPoints
        {
            get { return _FarmPoints; }
            set
            {
                _FarmPoints = value;
                OnPropertyChanged("FarmPoints");
            }
        }

        public ObservableCollection<Point> ForestPoints
        {
            get { return _ForestPoints; }
            set
            {
                _ForestPoints = value;
                OnPropertyChanged("ForestPoints");
            }
        }

        public ObservableCollection<Point> GrassPoints
        {
            get { return _GrassPoints; }
            set
            {
                _GrassPoints = value;
                OnPropertyChanged("GrassPoints");
            }
        }

        public ObservableCollection<Point> ShrubsPoints
        {
            get { return _ShrubsPoints; }
            set
            {
                _ShrubsPoints = value;
                OnPropertyChanged("ShrubsPoints");
            }
        }

        public ObservableCollection<Point> UnknownPoints
        {
            get { return _UnknownPoints; }
            set
            {
                _UnknownPoints = value;
                OnPropertyChanged("UnknownPoints");
            }
        }

        public ObservableCollection<Point> WaterIcePoints
        {
            get { return _WaterIcePoints; }
            set
            {
                _WaterIcePoints = value;
                OnPropertyChanged("WaterIcePoints");
            }
        }

        public ObservableCollection<Point> WetlandPoints
        {
            get { return _WetlandPoints; }
            set
            {
                _WetlandPoints = value;
                OnPropertyChanged("WetlandPoints");
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}