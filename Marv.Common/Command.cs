namespace Marv.Common
{
    public class Command<T> : NotifyPropertyChanged
    {
        private string imageSource;
        private bool isVisible = true;

        public string ImageSource
        {
            get
            {
                return this.imageSource;
            }

            set
            {
                if (value == this.imageSource) return;

                this.imageSource = value;
                this.RaisePropertyChanged("ImageSource");
            }
        }

        public bool IsVisible
        {
            get
            {
                return this.isVisible;
            }

            set
            {
                if (value != this.isVisible)
                {
                    this.isVisible = value;
                    this.RaisePropertyChanged("IsVisible");
                }
            }
        }

        public virtual void Excecute(T item)
        {
        }
    }
}