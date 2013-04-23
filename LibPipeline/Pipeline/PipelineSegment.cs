using LibBn;
using MongoDB.Bson;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace LibPipeline
{
    [Serializable]
    public class PipelineSegment : INotifyPropertyChanged, IPipelineLocation
    {
        public static int count = 0;

        private PipelineChemistry _Chemistry;
        private PipelineDateTime _DateTime;
        private string _Description;
        private double? _DistanceFromOrigin;
        private double? _Elevation;
        private ObjectId _Id;

        private double _Inclination;

        private ObservableCollection<PipelineSegmentInstant> _Instants;

        private bool _IsRiverCrossing;

        private bool _IsRoadCrossing;

        private double? _JointLength;

        private int _Landcover;

        private double? _Latitude;

        private double? _Longitude;

        private string _LongSeamOrientation;

        private string _Name;

        private double _PopulationDensity;

        private double? _Slope;

        private ObservableCollection<BnVertexValue> _VerticesThirdPartyDamage;

        private double? _WallThickness;

        public PipelineSegment()
        {
            this.IsRiverCrossing = false;
            this.IsRoadCrossing = false;

            this.Chemistry = new PipelineChemistry();
            this.DateTime = new PipelineDateTime();
            this.Description = "";
            this.DistanceFromOrigin = 0;
            this.Inclination = 0;
            this.Instants = new ObservableCollection<PipelineSegmentInstant>();
            this.Landcover = 0;
            this.LongSeamOrientation = "";
            this.Name = "";
            this.PopulationDensity = 0;
            this.VerticesThirdPartyDamage = new ObservableCollection<BnVertexValue>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public PipelineChemistry Chemistry
        {
            get { return _Chemistry; }
            set
            {
                _Chemistry = value;
                OnPropertyChanged("Chemistry");
            }
        }

        public PipelineDateTime DateTime
        {
            get { return _DateTime; }
            set
            {
                _DateTime = value;
                OnPropertyChanged("DateTime");
            }
        }

        [DisplayName("Description")]
        public string Description
        {
            get { return _Description; }
            set
            {
                _Description = value;
                OnPropertyChanged("Description");
            }
        }

        public double? DistanceFromOrigin
        {
            get { return _DistanceFromOrigin; }
            set
            {
                _DistanceFromOrigin = value;
                OnPropertyChanged("DistanceFromOrigin");
            }
        }

        [DisplayName("Elevation (m)")]
        public double? Elevation
        {
            get { return _Elevation; }
            set
            {
                _Elevation = value;
                OnPropertyChanged("Elevation");
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

        public double Inclination
        {
            get { return _Inclination; }
            set
            {
                _Inclination = value;
                OnPropertyChanged("Inclination");
            }
        }

        public ObservableCollection<PipelineSegmentInstant> Instants
        {
            get { return _Instants; }
            set
            {
                _Instants = value;
                OnPropertyChanged("Instants");
            }
        }

        public bool IsRiverCrossing
        {
            get { return _IsRiverCrossing; }
            set
            {
                _IsRiverCrossing = value;
                OnPropertyChanged("IsRiverCrossing");
            }
        }

        public bool IsRoadCrossing
        {
            get { return _IsRoadCrossing; }
            set
            {
                _IsRoadCrossing = value;
                OnPropertyChanged("IsRoadCrossing");
            }
        }

        [DisplayName("Joint Length (m)")]
        public double? JointLength
        {
            get { return _JointLength; }
            set
            {
                _JointLength = value;
                OnPropertyChanged("JointLength");
            }
        }

        public int Landcover
        {
            get { return _Landcover; }
            set
            {
                _Landcover = value;
                OnPropertyChanged("Landcover");
            }
        }

        public double? Latitude
        {
            get { return _Latitude; }
            set
            {
                _Latitude = value;
                OnPropertyChanged("Latitude");
            }
        }

        public double? Longitude
        {
            get { return _Longitude; }
            set
            {
                _Longitude = value;
                OnPropertyChanged("Longitude");
            }
        }

        [DisplayName("Long Seam Orientation")]
        public string LongSeamOrientation
        {
            get { return _LongSeamOrientation; }
            set
            {
                _LongSeamOrientation = value;
                OnPropertyChanged("LongSeamOrientation");
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

        public double PopulationDensity
        {
            get { return _PopulationDensity; }
            set
            {
                _PopulationDensity = value;
                OnPropertyChanged("PopulationDensity");
            }
        }

        [DisplayName("Slope (deg)")]
        public double? Slope
        {
            get { return _Slope; }
            set
            {
                _Slope = value;
                OnPropertyChanged("Slope");
            }
        }

        public ObservableCollection<BnVertexValue> VerticesThirdPartyDamage
        {
            get { return _VerticesThirdPartyDamage; }
            set
            {
                _VerticesThirdPartyDamage = value;
                OnPropertyChanged("VerticesThirdPartyDamage");
            }
        }

        [DisplayName("Wall Thickness")]
        public double? WallThickness
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