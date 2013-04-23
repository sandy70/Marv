using System;
using System.ComponentModel;

namespace LibPipeline
{
    [Serializable]
    public class PipelineFlowModel : INotifyPropertyChanged
    {
        private double _FlowIndex;

        private double _InclinationCritical;

        private double _Pressure;

        private double _RateMean;

        private double _Temperature;

        private double _Velocity;

        public PipelineFlowModel()
        {
            this.FlowIndex = 1;
            this.InclinationCritical = 0;
            this.Pressure = 5;
            this.RateMean = 100;
            this.Temperature = 50;
            this.Velocity = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public double FlowIndex
        {
            get { return _FlowIndex; }
            set
            {
                _FlowIndex = value;
                OnPropertyChanged("FlowIndex");
            }
        }

        public double InclinationCritical
        {
            get { return _InclinationCritical; }
            set
            {
                _InclinationCritical = value;
                OnPropertyChanged("InclinationCritical");
            }
        }

        public double Pressure
        {
            get { return _Pressure; }
            set
            {
                _Pressure = value;
                OnPropertyChanged("Pressure");
            }
        }

        public double RateMean
        {
            get { return _RateMean; }
            set
            {
                _RateMean = value;
                OnPropertyChanged("RateMean");
            }
        }

        public double Temperature
        {
            get { return _Temperature; }
            set
            {
                _Temperature = value;
                OnPropertyChanged("Temperature");
            }
        }

        public double Velocity
        {
            get { return _Velocity; }
            set
            {
                _Velocity = value;
                OnPropertyChanged("Velocity");
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}