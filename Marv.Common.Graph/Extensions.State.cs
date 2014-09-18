using System.Collections.Generic;
using System.Linq;

namespace Marv
{
    public static partial class Extensions
    {
        public static void ClearEvidence(this IEnumerable<State> states)
        {
            foreach (var state in states) state.Evidence = 0;
        }

        public static IEnumerable<double> GetEvidence(this IEnumerable<State> states)
        {
            return states.Select(state => state.Evidence);
        }

        public static IEnumerable<double> Parse(this IEnumerable<State> states, IDistribution dist)
        {
            return states.Select(state => dist.Cdf(state.SafeMax) - dist.Cdf(state.SafeMin));
        }

        public static IEnumerable<double> Parse(this IEnumerable<State> states, string str)
        {
            var stateList = states as IList<State> ?? states.ToList();

            if (string.IsNullOrWhiteSpace(str)) return null;

            // Check if string is the label of any of the states.
            if (stateList.Any(state => state.Key == str))
            {
                return stateList.Select(state => state.Key == str ? 1.0 : 0.0);
            }

            var paramValues = VertexData.ParseEvidenceParams(str);

            // Check for functions
            if (str.ToLowerInvariant().Contains("tri") && paramValues.Count == 3)
            {
                return stateList.Parse(new TriangularDistribution(paramValues[0], paramValues[1], paramValues[2]));
            }

            if (str.ToLowerInvariant().Contains("norm") && paramValues.Count == 2)
            {
                return stateList.Parse(new NormalDistribution(paramValues[0], paramValues[1]));
            }

            if (str.Contains(":") && paramValues.Count == 2)
            {
                return stateList.Parse(new UniformDistribution(paramValues[0], paramValues[1]));
            }

            if (str.Contains(",") && paramValues.Count == stateList.Count())
            {
                return paramValues;
            }

            if (paramValues.Count == 1)
            {
                return stateList.Parse(new DeltaDistribution(paramValues[0]));
            }

            return null;
        }

        public static void SetBelief(this IEnumerable<State> states, double belief)
        {
            foreach (var state in states) state.Belief = belief;
        }
    }
}