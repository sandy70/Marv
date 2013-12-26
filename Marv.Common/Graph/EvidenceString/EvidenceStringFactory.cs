using System.Linq;

namespace Marv.Common
{
    public static class EvidenceStringFactory
    {
        public static EvidenceString Create(string evidenceString)
        {
            if (evidenceString.Contains(';'))
            {
                return new DistributionEvidenceString(evidenceString);
            }
            else if (evidenceString.Contains(':'))
            {
                return new RangeEvidenceString(evidenceString);
            }
            else
            {
                return new StateEvidenceString(evidenceString);
            }
        }
    }
}