using Marv.Common;
using System;

namespace LibNetwork
{
    public class VertexCommand : ViewModel, IVertexCommand
    {
        public static VertexCommand VertexClearCommand = new VertexCommand
        {
            ImageSource = "/Marv.Common;component/Resources/Icons/Clear.png"
        };

        public static VertexExpandCommand VertexExpandCommand = new VertexExpandCommand
        {
            ImageSource = "/Marv.Common;component/Resources/Icons/Expand.png"
        };

        public static VertexLockCommand VertexLockCommand = new VertexLockCommand
        {
            ImageSource = "/Marv.Common;component/Resources/Icons/Lock.png"
        };

        public static VertexCommand VertexSubGraphCommand = new VertexCommand
        {
            ImageSource = "/Marv.Common;component/Resources/Icons/SubGraph.png"
        };

        private string imageSource;

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

        public virtual void Execute(VertexViewModel vertexViewModel)
        {
            if (this.Executed != null)
            {
                this.Executed(this, vertexViewModel);
            }
        }
    }
}