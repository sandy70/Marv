using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marv.Epri
{
    public class Stream
    {
        public string id { get; set; }
        public string type { get; set; }
        public string value { get; set; }
        public string timestamp { get; set; }
        public string server_timestamp { get; set; }
        public string history_uri { get; set; }
        public string units { get; set; }
    }
}
