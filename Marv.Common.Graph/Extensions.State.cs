using System.Collections.Generic;
using System.Linq;
using Marv.Common;

namespace Marv
{
    public static partial class Extensions
    {
        public static double GetSafeMax(this IEnumerable<State> states)
        {
            return states.Max(state => state.SafeMax);
        }

        public static double GetSafeMin(this IEnumerable<State> states)
        {
            return states.Min(state => state.SafeMin);
        }

        public static double[] ParseEvidence(this IEnumerable<State> states, IDistribution dist)
        {
            return states.Select(state => dist.Cdf(state.SafeMax + Utils.Epsilon) - dist.Cdf(state.SafeMin - Utils.Epsilon)).Normalized().ToArray();
        }

        public static VertexEvidence ParseEvidenceString(this IEnumerable<State> states, string anEvidenceString)
        {
            if (string.IsNullOrWhiteSpace(anEvidenceString))
            {
                return new VertexEvidence
                {
                    Type = VertexEvidenceType.Null
                };
            }

            var stateList = states as IList<State> ?? states.ToList();

            // Check if string is the label of any of the states.
            if (stateList.Any(state => state.Key == anEvidenceString))
            {
                return new VertexEvidence
                {
                    Value = stateList.Select(state => state.Key == anEvidenceString ? 1.0 : 0.0).Normalized().ToArray(),
                    Type = VertexEvidenceType.State,
                    StateKey = stateList.Where(state => state.Key == anEvidenceString).Select(state => state.Key).First()
                };
            }

            double value;
            if (double.TryParse(anEvidenceString, out value) && stateList.GetSafeMin() <= value && value <= stateList.GetSafeMax())
            {
                return new VertexEvidence
                {
                    Value = stateList.ParseEvidence(new DeltaDistribution(value)),
                    Type = VertexEvidenceType.Number,
                    Params = new[]
                    {
                        value
                    },
                };
            }

            var evidenceParams = VertexEvidence.ParseEvidenceParams(anEvidenceString);
            var evidenceType = VertexEvidenceType.Invalid;
            double[] evidence = null;

            // Check for functions
            if (anEvidenceString.ToLowerInvariant().Contains("tri") && evidenceParams.Count == 3)
            {
                evidenceParams.Sort();

                evidence = stateList.ParseEvidence(new TriangularDistribution(evidenceParams[0], evidenceParams[1], evidenceParams[2]));
                evidenceType = VertexEvidenceType.Triangular;
            }

            else if (anEvidenceString.ToLowerInvariant().Contains("norm") && evidenceParams.Count == 2)
            {
                evidence = stateList.ParseEvidence(new NormalDistribution(evidenceParams[0], evidenceParams[1]));
                evidenceType = VertexEvidenceType.Normal;
            }

            else if (anEvidenceString.Contains(":") && evidenceParams.Count == 2)
            {
                evidenceParams.Sort();

                evidence = stateList.ParseEvidence(new UniformDistribution(evidenceParams[0], evidenceParams[1]));
                evidenceType = VertexEvidenceType.Range;
            }

            else if (evidenceParams.Count == stateList.Count)
            {
                evidence = evidenceParams.Normalized().ToArray();
                evidenceType = VertexEvidenceType.Distribution;
            }

            return new VertexEvidence
            {
                Value = evidence,
                Type = evidenceType,
                Params = evidenceParams.ToArray(),
            };
        }
    }
}