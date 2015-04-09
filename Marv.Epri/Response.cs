using System.Collections.Generic;

namespace Marv.Epri
{
    public class Response
    {
        public int count { get; set; }
        public string cursor { get; set; }
        public List<DataPoint> list { get; set; }
        public string next_uri { get; set; }
        public int size { get; set; }
    }
}