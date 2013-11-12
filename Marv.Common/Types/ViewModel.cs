using System.ComponentModel;

namespace Marv.Common
{
    public interface IViewModel : INotifyPropertyChanged
    {
        string Key { get; set; }

        string Name { get; set; }
    }

    public class ViewModel : Dynamic, IViewModel
    {
        private string key = "";
        private string name = "";

        public event PropertyChangedEventHandler PropertyChanged;

        public string Key
        {
            get
            {
                return this.key;
            }

            set
            {
                if (value != this.key)
                {
                    this.key = value;
                    this.RaisePropertyChanged("Key");
                }
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                if (value != this.name)
                {
                    this.name = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}