using System;
using System.ComponentModel;

namespace LibPipeline
{
    [Serializable]
    public class PipelineSegmentInstant : INotifyPropertyChanged
    {
        private PipelineCorrosionModel corrosionModel;

        private DateTime date;

        private PipelineFlowModel flowModel;

        public PipelineSegmentInstant()
        {
            this.CorrosionModel = new PipelineCorrosionModel();
            this.Date = DateTime.Now;
            this.FlowModel = new PipelineFlowModel();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public PipelineCorrosionModel CorrosionModel
        {
            get { return corrosionModel; }
            set
            {
                corrosionModel = value;
                OnPropertyChanged("CorrosionModel");
            }
        }

        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value;
                OnPropertyChanged("Date");
            }
        }

        public PipelineFlowModel FlowModel
        {
            get { return flowModel; }
            set
            {
                flowModel = value;
                OnPropertyChanged("FlowModel");
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}