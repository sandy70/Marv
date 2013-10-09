using Marv.Common;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marv.Controls
{
    public class NotificationIndeterminate : ViewModel, INotification
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private double _value = 100;
        private string description = "";
        private bool isIndeterminate = true;
        private string name = "";

        public event EventHandler Stopped;

        public string Description
        {
            get
            {
                return this.description;
            }

            set
            {
                if (value != this.description)
                {
                    this.description = value;
                    this.RaisePropertyChanged("Description");
                }
            }
        }

        public bool IsIndeterminate
        {
            get
            {
                return this.isIndeterminate;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                if (value != this.name)
                {
                    this.name = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }

        public double Value
        {
            get
            {
                return this._value;
            }

            private set
            {
                if (value != this._value)
                {
                    this._value = value;
                    this.RaisePropertyChanged("Value");
                }
            }
        }

        public void Start()
        {
            logger.Trace("");
        }

        public void Stop()
        {
            if (this.Stopped != null)
            {
                this.Stopped(this, new EventArgs());
            }
        }
    }
}
