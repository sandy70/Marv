using Marv.Common;
using System;

namespace LibNetwork
{
    public class VertexCommand : ViewModel, IVertexCommand
    {
        private string imageSource;
        private bool isVisible = true;

        public event EventHandler<VertexViewModel> Executed;

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
                    this.RaisePropertyChanged("ImageSource");
                }
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

        public void Execute(VertexViewModel vertexViewModel)
        {
            if (this.Executed != null)
            {
                this.Executed(this, vertexViewModel);
            }
        }
    }
}