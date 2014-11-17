namespace Marv
{
    public class Command<T> : NotifyPropertyChanged
    {
        private string imageSource;

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
                this.RaisePropertyChanged();
            }
        }

        public virtual void Excecute(T item) {}
    }
}