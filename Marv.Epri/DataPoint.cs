using System;

namespace Marv.Epri
{
    public class DataPoint
    {
        public string id { get; set; }
        public int quality { get; set; }
        public DateTime server_timestamp { get; set; }
        public string stream_id { get; set; }
        public DateTime timestamp { get; set; }
        public int value { get; set; }
    }
}