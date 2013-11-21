using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marv.Common
{
    public abstract class EvidenceString
    {
        protected string _string = null;

        public EvidenceString(string aString)
        {
            this._string = aString;
        }

        public abstract IEvidence Parse(Vertex vertex);
    }
}
