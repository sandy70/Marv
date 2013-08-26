using LibNetwork;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace LibPipeline
{
    [Serializable]
    public class PipelineCorrosionModel : INotifyPropertyChanged
    {
        private double _ProbabilityOfWater;

        private double _Rate;

        private double _Thickness;

        private ObservableCollection<BnVertexValue> _Vertices;

        public PipelineCorrosionModel()
        {
            this.ProbabilityOfWater = 0;
            this.Rate = 0;
            this.Thickness = 0;
            this.Vertices = new ObservableCollection<BnVertexValue>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public double ProbabilityOfWater
        {
            get { return _ProbabilityOfWater; }
            set
            {
                _ProbabilityOfWater = value;
                OnPropertyChanged("ProbabilityOfWater");
            }
        }

        public double Rate
        {
            get { return _Rate; }
            set
            {
                _Rate = value;
                OnPropertyChanged("Rate");
            }
        }

        public double Thickness
        {
            get { return _Thickness; }
            set
            {
                _Thickness = value;
                OnPropertyChanged("Thickness");
            }
        }

        public ObservableCollection<BnVertexValue> Vertices
        {
            get { return _Vertices; }
            set
            {
                _Vertices = value;
                OnPropertyChanged("Vertices");
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}