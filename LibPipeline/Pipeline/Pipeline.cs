using MongoDB.Bson;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace LibPipeline
{
    [Serializable]
    public class Pipeline : INotifyPropertyChanged
    {
        private string _Collection;

        private string _Name;

        private PipelineProperties _Properties;

        private ObservableCollection<PipelineSegment> _Segments;

        public Pipeline()
        {
            this.Collection = ObjectId.GenerateNewId(DateTime.Now).ToString();
            this.Name = "";
            this.Properties = new PipelineProperties();
            this.Segments = new ObservableCollection<PipelineSegment>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Collection
        {
            get { return _Collection; }
            set
            {
                _Collection = value;
                OnPropertyChanged("Collection");
            }
        }

        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                OnPropertyChanged("Name");
            }
        }

        public PipelineProperties Properties
        {
            get { return _Properties; }
            set
            {
                _Properties = value;
                OnPropertyChanged("Properties");
            }
        }

        public ObservableCollection<PipelineSegment> Segments
        {
            get { return _Segments; }
            set
            {
                _Segments = value;
                OnPropertyChanged("Segments");
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}