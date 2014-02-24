namespace Marv.Common.Graph
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
