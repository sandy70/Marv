using System;

namespace Marv.Common
{
    public interface INotification
    {
        event EventHandler Closed;

        string Description { get; set; }

        bool IsIndeterminate { get; }

        string Name { get; set; }

        double Value { get; }

        void Close();

        void Open();
    }
}