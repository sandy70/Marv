using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Marv.Common.Graph
{
    internal class TriEvidenceString : IEvidenceStringParser
    {
        public Dictionary<string, double> Parse(IEnumerable<State> states, string str)
        {
            var evidence = new Dictionary<string, double>();

            // Gets the values between ( and )
            var parts = Regex.Match(str, @"\(([^)]*)\)").Groups[1].Value
                .Trim()
                .Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);

            // we need exactly 3 parts
            if (parts.Count() != 3) return evidence;

            double min, mod, max;

            if (!double.TryParse(parts[0], out min)) return evidence;
            if (!double.TryParse(parts[1], out mod)) return evidence;
            if (!double.TryParse(parts[2], out max)) return evidence;

            // If the parameters are not in the right order
            if (!(min < mod && mod < max)) return evidence;

            var triangularDistribution = new TriangularDistribution(min, mod, max);

            foreach (var state in states)
            {
                evidence[state.Key] = triangularDistribution.Cdf(state.Max) - triangularDistribution.Cdf(state.Min);
            }

            return evidence.Normalized();
        }
    }
}