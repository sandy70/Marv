using System.Linq;

namespace Marv.Common.Graph
{
    public static class EvidenceStringFactory
    {
        public static IEvidenceStringParser Create(string evidenceString)
        {
            if (evidenceString == null)
            {
                return new NullEvidenceString();
            }

            if (evidenceString.Contains(';'))
            {
                return new DistributionEvidenceString();
            }

            if (evidenceString.Contains(':'))
            {
                return new RangeEvidenceString();
            }

            return new StateEvidenceString();
        }
    }
}