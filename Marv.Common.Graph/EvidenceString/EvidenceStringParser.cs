using System.Collections.Generic;

namespace Marv.Common.Graph
{
    public abstract class EvidenceStringParser
    {
        protected readonly string _string = null;

        protected EvidenceStringParser(string aString)
        {
            this._string = aString;
        }

        public abstract Dictionary<string, double> Parse(Vertex vertex);
    }
}