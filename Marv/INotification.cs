using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marv
{
    public interface INotification
    {
        string Name { get; set; }
        string Description { get; set; }
        double Value { get; }
        bool IsIndeterminate { get; }

        void OnAdded();
    }
}
