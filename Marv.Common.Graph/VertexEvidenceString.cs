using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marv.Common.Graph
{
    public class VertexEvidenceString : IVertexEvidence
    {
        private readonly string _string;

        public VertexEvidenceString(string aString)
        {
            this._string = aString;
        }

        public virtual bool Set(Vertex vertex)
        {
            vertex.EvidenceString = this._string;
            var evidence = EvidenceStringFactory.Create(this._string).Parse(vertex.States, this._string);

            if (evidence != null)
            {
                vertex.Evidence = evidence;
                return true;
            }

            return false;
        }
    }
}
