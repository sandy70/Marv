using System.Collections.Generic;

namespace Marv.Common.Graph
{
    public abstract class EvidenceString
    {
        protected string _string = null;

        public EvidenceString(string aString)
        {
            this._string = aString;
        }

        public abstract Dictionary<string, double> Parse(Vertex vertex);
    }
}