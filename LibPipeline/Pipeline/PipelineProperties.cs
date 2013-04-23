using MongoDB.Bson;
using System;
using System.ComponentModel;

namespace LibPipeline
{
    [Serializable]
    public class PipelineProperties : INotifyPropertyChanged
    {
        private string _Commodity;
        private double _Diameter;
        private ObjectId _Id;

        private double _Inhibitor;

        private double _WallThickness;

        public PipelineProperties()
        {
            this.Commodity = "";
            this.Diameter = 0;
            this.Inhibitor = 0;
            this.WallThickness = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Commodity
        {
            get { return _Commodity; }
            set
            {
                _Commodity = value;
                OnPropertyChanged("Commodity");
            }
        }

        public double Diameter
        {
            get { return _Diameter; }
            set
            {
                _Diameter = value;
                OnPropertyChanged("Diameter");
            }
        }

        public ObjectId Id
        {
            get { return _Id; }
            set
            {
                _Id = value;
                OnPropertyChanged("Id");
            }
        }

        public double Inhibitor
        {
            get { return _Inhibitor; }
            set
            {
                _Inhibitor = value;
                OnPropertyChanged("Inhibitor");
            }
        }

        public double WallThickness
        {
            get { return _WallThickness; }
            set
            {
                _WallThickness = value;
                OnPropertyChanged("WallThickness");
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}