using System;
using System.Collections.Generic;
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

            if (evidenceString.Contains("TRI"))
            {
                return new TriEvidenceString();
            }

            if (evidenceString.Contains("NORM"))
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
            var parts = Regex.Match(str, @"\(([^)]*)\)").Groups[1].Value
                .Trim()
                .Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);

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