using System;
using System.ComponentModel;

namespace Marv.Common
{
    public interface IVertexCommand : INotifyPropertyChanged
    {
        event EventHandler<Vertex> Executed;

        string ImageSource { get; set; }

        void RaiseExecuted(Vertex vertexViewModel);
    }
}