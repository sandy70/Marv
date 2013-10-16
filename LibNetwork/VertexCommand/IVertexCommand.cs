using System;
using System.ComponentModel;

namespace LibNetwork
{
    public interface IVertexCommand : INotifyPropertyChanged
    {
        event EventHandler<VertexViewModel> Executed;

        string ImageSource { get; set; }

        bool IsVisible { get; set; }

        void Execute(VertexViewModel vertexViewModel);
    }
}