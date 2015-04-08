using System.Collections.Generic;

namespace Marv.LightSensor
{
    internal class XivelyFeed
    {
        public string created { get; set; }
        public string creator { get; set; }
        public List<Datastream> DataStreams { get; set; }
        public string device_serial { get; set; }
        public string feed { get; set; }
        public int id { get; set; }
        public string @private { get; set; }
        public string product_id { get; set; }
        public string status { get; set; }
        public string title { get; set; }
        public string updated { get; set; }
        public string version { get; set; }
    }
}