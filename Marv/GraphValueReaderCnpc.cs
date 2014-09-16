using System;
using System.IO;
using Marv;

namespace Marv
{
    public class GraphValueReaderCnpc : IGraphValueReader
    {
        private Dictionary<int, string, string, double> graphValues;

        public string InputDir { get; set; }

        public Marv SourceGraph { get; set; }

        public Dictionary<int, string, string, double> Read(string lineKey, string locationKey)
        {
            if (this.graphValues == null)
            {
                this.graphValues = new Dictionary<int, string, string, double>();

                foreach (var fileName in Directory.GetFiles(this.InputDir, "*.vertices"))
                {
                    var year = Int32.Parse(Path.GetFileNameWithoutExtension(fileName));
                    this.graphValues[year] = this.SourceGraph.ReadValueCsv(fileName);
                }
            }

            return this.graphValues;
        }
    }
}