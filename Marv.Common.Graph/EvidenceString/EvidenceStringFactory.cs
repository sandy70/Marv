using System;
using System.Linq;
using System.Text.RegularExpressions;

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

            if (evidenceString.ToLowerInvariant().Contains("tri"))
            {
                return new TriEvidenceString();
            }

            if (evidenceString.ToLowerInvariant().Contains("norm"))
            {
                return new NormEvidenceString();
            }

            if (evidenceString.Contains(','))
            {
                return new DistributionEvidenceString();
            }

            if (evidenceString.Contains(':'))
            {
                return new RangeEvidenceString();
            }

            return new StateEvidenceString();
        }

        public static double[] ParseParams(string str)
        {
            // Gets the values between ( and )
            return EvidenceStringFactory.ParseArray(Regex.Match(str, @"\(([^)]*)\)").Groups[1].Value);
        }

        public static double[] ParseArray(string str, string delims = ",")
        {
            var parts = str.Trim()
                .Split(delims.ToArray(), StringSplitOptions.RemoveEmptyEntries);

            var values = new double[parts.Count()];

            for (var i = 0; i < parts.Length; i++)
            {
                // Return null if any of these cannot be parsed
                if (!double.TryParse(parts[i], out values[i])) return null;
            }

            return values;
        }
    }
}