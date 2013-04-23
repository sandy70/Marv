using System;
using System.ComponentModel;

namespace LibPipeline
{
    [Serializable]
    public class PipelineChemistry : INotifyPropertyChanged
    {
        private double _CO2;

        private double _Fe2Plus;

        private double _H2S;

        private double _HAc;

        private double _O2;

        private double _pH;

        public PipelineChemistry()
        {
            this.CO2 = 0;
            this.H2S = 0;
            this.Fe2Plus = 0;
            this.HAc = 0;
            this.pH = 0;
            this.O2 = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public double CO2
        {
            get { return _CO2; }
            set
            {
                _CO2 = value;
                OnPropertyChanged("CO2");
            }
        }

        public double Fe2Plus
        {
            get { return _Fe2Plus; }
            set
            {
                _Fe2Plus = value;
                OnPropertyChanged("Fe2Plus");
            }
        }

        public double H2S
        {
            get { return _H2S; }
            set
            {
                _H2S = value;
                OnPropertyChanged("H2S");
            }
        }

        public double HAc
        {
            get { return _HAc; }
            set
            {
                _HAc = value;
                OnPropertyChanged("HAc");
            }
        }

        public double O2
        {
            get { return _O2; }
            set
            {
                _O2 = value;
                OnPropertyChanged("O2");
            }
        }

        public double pH
        {
            get { return _pH; }
            set
            {
                _pH = value;
                OnPropertyChanged("pH");
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}