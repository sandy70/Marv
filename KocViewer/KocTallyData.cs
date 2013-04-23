using System.ComponentModel;

namespace KocViewer
{
    public class KocTallyData : INotifyPropertyChanged
    {
        private double elevation;
        private double jointLength;

        public event PropertyChangedEventHandler PropertyChanged;

        public double Elevation
        {
            get
            {
                return this.elevation;
            }

            set
            {
                if (value != this.elevation)
                {
                    this.elevation = value;
                    this.OnPropertyChanged("Elevation");
                }
            }
        }

        public double JointLength
        {
            get
            {
                return this.jointLength;
            }

            set
            {
                if (value != this.jointLength)
                {
                    this.jointLength = value;
                    this.OnPropertyChanged("JointLength");
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