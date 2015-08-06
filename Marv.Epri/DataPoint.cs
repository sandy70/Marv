using System;
using Marv.Common;

namespace Marv.Epri
{
    // Do not change the property names here because they must match the JSON response
    public class DataPoint : NotifyPropertyChanged
    {
        private DateTime server_timestamp;
        private int value;

        public DateTime Server_TimeStamp
        {
            get { return this.server_timestamp; }

            set
            {
                if (value.Equals(this.server_timestamp))
                {
                    return;
                }

                this.server_timestamp = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged("Local_TimeStamp");
            }
        }

        public DateTime Local_TimeStamp
        {
            get { return this.Server_TimeStamp + (DateTime.Now - DateTime.UtcNow); }
        }

        public int Value
        {
            get { return this.value; }

            set
            {
                if (value.Equals(this.value))
                {
                    return;
                }

                this.value = value;
                this.RaisePropertyChanged();
            }
        }

        public string id { get; set; }
        public int quality { get; set; }
        public string stream_id { get; set; }
        public DateTime timestamp { get; set; }
    }
}