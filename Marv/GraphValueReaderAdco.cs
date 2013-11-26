using Marv.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marv
{
    public class GraphValueReaderAdco : IGraphValueReader
    {
        public Dict<int, string, string, double> Read(string lineKey, string locationKey)
        {
            try
            {
                var fileName = this.GetFileNameForModelValue(lineKey, locationKey);
                return Odb.ReadValueSingle<Dict<int, string, string, double>>(fileName, x => true);
            }
            catch(OdbDataNotFoundException exception)
            {
                throw new GraphValueNotFoundException("Value not found for location: " + locationKey + " on line: " + lineKey, exception);
            }
        }

        public string GetFileNameForModelValue(string lineKey, string locationKey)
        {
            return Path.Combine("ModelValues", lineKey, locationKey + ".db");
        }
    }
}
