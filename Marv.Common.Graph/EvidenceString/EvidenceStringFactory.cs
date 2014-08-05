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

        /// <summary>
        ///     Parses a delimited set of values into a double[]. If any of the values cannot be converted to double, returns null.
        /// </summary>
        /// <param name="str">The string to be parsed. e.g. "1.2,3.4,5.6" or "11.2:-4:7.9</param>
        /// <param name="delims">The delimiteres separating the values.</param>
        /// <returns>Parsed array of doubles or null</returns>
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

        /// <summary>
        ///     Parses a string of type "FUNC(1.0, 2.0, 3.0, ...)" to get the list of values passed as parameters. If any of the
        ///     values cannot be converted to double, returns null.
        /// </summary>
        /// <param name="str">The string to be parsed</param>
        /// <returns>Parsed array of doubles or null.</returns>
        public static double[] ParseParams(string str)
        {
            // Gets the values between ( and )
            return ParseArray(Regex.Match(str, @"\(([^)]*)\)").Groups[1].Value);
        }
    }
}