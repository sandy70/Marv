namespace Marv
{
    public class VertexCommand : NotifyPropertyChanged
    {
        public static readonly VertexCommand VertexClearCommand = new VertexCommand
        {
            ImageSource = "/Marv.Common;component/Resources/Icons/Clear.png"
        };

        private string imageSource;

        public string ImageSource
        {
            get
            {
                return this.imageSource;
            }

            set
            {
                if (value != this.imageSource)
                {
                    this.imageSource = value;
                    this.RaisePropertyChanged();
                }
            }
        }
    }
}