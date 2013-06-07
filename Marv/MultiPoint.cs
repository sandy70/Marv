using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace Marv
{
    public class MultiPoint : INotifyPropertyChanged
    {
        private string name;
        private ObservableCollection<Point> points = new ObservableCollection<Point>();

        public event PropertyChangedEventHandler PropertyChanged;

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

                    this.OnPropertyChanged("Name");
                }
            }
        }

        public ObservableCollection<Point> Points
        {
            get
            {
                return this.points;
            }

            set
            {
                if (value != this.points)
                {
                    this.points = value;

                    this.OnPropertyChanged("Points");
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}