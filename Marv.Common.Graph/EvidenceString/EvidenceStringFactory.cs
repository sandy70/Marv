using System.Linq;

namespace Marv.Common.Graph
{
    public static class EvidenceStringFactory
    {
        public static EvidenceString Create(string evidenceString)
        {
            if (evidenceString == null) return new NullEvidenceString(null);

            if (evidenceString.Contains(';'))
            {
                return new DistributionEvidenceString(evidenceString);
            }

            if (evidenceString.Contains(':'))
            {
                return new RangeEvidenceString(evidenceString);
            }

            return new StateEvidenceString(evidenceString);
        }
    }
}