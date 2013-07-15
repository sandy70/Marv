using System.ComponentModel;

namespace LibPipeline
{
    public class MultiLocation : SelectableCollection<Location>
    {
        private string name;

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
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Name"));
                }
            }
        }
    }
}