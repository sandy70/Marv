using LibPipeline;
using System.ComponentModel;

namespace Marv
{
    public class InfoWindowViewModel : INotifyPropertyChanged
    {
        private PipelineViewModel _PipelineViewModel;

        public InfoWindowViewModel()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public PipelineViewModel PipelineViewModel
        {
            get { return _PipelineViewModel; }
            set
            {
                _PipelineViewModel = value;
                OnPropertyChanged("PipelineViewModel");
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}