using System;

namespace Marv.Common.Graph
{
    public class VertexCommand : Model, IVertexCommand
    {
        public static VertexCommand VertexClearCommand = new VertexCommand
        {
            ImageSource = "/Marv.Common;component/Resources/Icons/Clear.png"
        };

        private string imageSource;

        public event EventHandler<Vertex> Executed;

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

        public virtual void RaiseExecuted(Vertex vertexViewModel)
        {
            if (this.Executed != null)
            {
                this.Executed(this, vertexViewModel);
            }
        }
    }
}