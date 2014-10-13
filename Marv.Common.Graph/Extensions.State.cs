﻿using System.Collections.Generic;
using System.Linq;

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
            return states.Max(state => state.SafeMin);
        }

        public static IEnumerable<double> Parse(this IEnumerable<State> states, IDistribution dist)
        {
            return states.Select(state => dist.Cdf(state.SafeMax) - dist.Cdf(state.SafeMin));
        }

        public static IEnumerable<double> Parse(this IEnumerable<State> states, string str)
        {
            var stateList = states as IList<State> ?? states.ToList();

            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }

            // Check if string is the label of any of the states.
            if (stateList.Any(state => state.Key == str))
            {
                return stateList.Select(state => state.Key == str ? 1.0 : 0.0);
            }

            var paramValues = VertexEvidence.ParseEvidenceParams(str);

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

        public static double[] ParseEvidence(this IEnumerable<State> states, IDistribution dist)
        {
            return states.Select(state => dist.Cdf(state.SafeMax) - dist.Cdf(state.SafeMin)).Normalized().ToArray();
        }

        public static VertexEvidence ParseEvidenceString(this IEnumerable<State> states, string anEvidenceString)
        {
            if (string.IsNullOrWhiteSpace(anEvidenceString))
            {
                return new VertexEvidence
                {
                    Type = VertexEvidenceType.Invalid
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

            if (anEvidenceString.ToLowerInvariant().Contains("norm") && evidenceParams.Count == 2)
            {
                evidence = stateList.ParseEvidence(new NormalDistribution(evidenceParams[0], evidenceParams[1]));
                evidenceType = VertexEvidenceType.Normal;
            }

            if (anEvidenceString.Contains(":") && evidenceParams.Count == 2)
            {
                evidenceParams.Sort();

                evidence = stateList.ParseEvidence(new UniformDistribution(evidenceParams[0], evidenceParams[1]));
                evidenceType = VertexEvidenceType.Range;
            }

            if (anEvidenceString.Contains(",") && evidenceParams.Count == stateList.Count)
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