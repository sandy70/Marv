using MapControl;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace LibPipeline
{
    [Serializable]
    public class Pipeline : INotifyPropertyChanged
    {
        private string _Collection;
        private PipelineProperties _Properties;
        private ObservableCollection<PipelineSegment> _Segments;
        private List<Location> locations = new List<Location>();
        private Dictionary<Location, dynamic> locationsToProperties = new Dictionary<Location,dynamic>();
        private string name;

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

        public List<Location> Locations
        {
            get
            {
                return this.locations;
            }

            set
            {
                if (value != this.locations)
                {
                    this.locations = value;

                    this.OnPropertyChanged("Locations");
                }
            }
        }

        public Dictionary<Location, dynamic> LocationsToProperties
        {
            get
            {
                return this.locationsToProperties;
            }

            set
            {
                if (value != this.locationsToProperties)
                {
                    this.locationsToProperties = value;
                    this.OnPropertyChanged("LocationsToProperties");
                }
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
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