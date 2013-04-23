using System.IO;

namespace LibConfig
{
    public class MongoInfo
    {
        private string _BinDir;

        public string BinDir
        {
            get
            {
                return _BinDir;
            }

            set
            {
                DExec = Path.Combine(value, "mongod.exe");
                DumpExec = Path.Combine(value, "mongodump.exe");
                _BinDir = value;
            }
        }

        public string Collection { get; set; }

        public string Connection { get; set; }

        public string Database { get; set; }

        public string DbPath { get; set; }

        public string DExec { get; set; }

        public string DumpExec { get; set; }
    }
}