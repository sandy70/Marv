using System;

namespace Marv.Common
{
    public interface INotification
    {
        event EventHandler Stopped;

        string Description { get; set; }

        bool IsIndeterminate { get; }

        string Name { get; set; }

        double Value { get; }

        void Start();

        void Stop();
    }
}