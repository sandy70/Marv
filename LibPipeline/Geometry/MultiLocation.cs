using System.ComponentModel;

namespace LibPipeline
{
    public class MultiLocation : SelectableCollection<Location>
    {
        private bool isEnabled = true;
        private bool isSelected = false;
        private string name = "";

        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }

            set
            {
                if (value != this.isEnabled)
                {
                    this.isEnabled = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("IsEnabled"));

                    if (!this.IsEnabled)
                    {
                        this.IsSelected = false;
                    }
                }
            }
        }

        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }

            set
            {
                if (this.IsEnabled)
                {
                    this.isSelected = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("IsSelected"));
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
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Name"));
                }
            }
        }
    }
}