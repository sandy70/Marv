using System.Collections.Generic;

namespace Marv.Epri
{
    // Do not change the property names here because they must match the JSON response
    public class Response
    {
        public int count { get; set; }
        public string cursor { get; set; }
        public List<DataPoint> list { get; set; }
        public string next_uri { get; set; }
        public int size { get; set; }
    }
}