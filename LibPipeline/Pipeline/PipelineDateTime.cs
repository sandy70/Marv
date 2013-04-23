using System;
using System.ComponentModel;

namespace LibPipeline
{
    [Serializable]
    public class PipelineDateTime : INotifyPropertyChanged
    {
        private DateTime[] _InstallationDates;

        private TimeSpan _Step;

        public PipelineDateTime()
        {
            this.InstallationDates = new DateTime[1];
            this.Step = new DateTime(2, 1, 1) - new DateTime(1, 1, 1);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public DateTime[] InstallationDates
        {
            get { return _InstallationDates; }
            set
            {
                _InstallationDates = value;
                OnPropertyChanged("InstallationDates");
            }
        }

        public TimeSpan Step
        {
            get { return _Step; }
            set
            {
                _Step = value;
                OnPropertyChanged("Step");
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}