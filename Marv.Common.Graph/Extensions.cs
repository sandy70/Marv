using System;
using System.Collections.Generic;
using System.Linq;

namespace Marv.Common.Graph
{
    public static partial class Extensions
    {
        public static void AddUnique(this ModelCollection<Edge> edges, Vertex source, Vertex target, EdgeConnectorPositions connectorPostions = null)
        {
            var newEdge = new Edge(source, target)
            {
                ConnectorPositions = connectorPostions ?? new EdgeConnectorPositions()
            };

            if (Contains(edges, source, target))
            {
                return;
            }

            edges.Add(newEdge);
        }

        public static bool Contains(this IEnumerable<Edge> edges, Vertex source, Vertex target)
        {
            return edges.Any(edge => edge.Source == source && edge.Target == target);
        }

        public static Dictionary<string, double> GetBelief(this IEnumerable<State> states)
        {
            return states.ToDictionary(state => state.Key, state => state.Belief);
        }

        public static Dictionary<string, double> GetEvidence(this IEnumerable<State> states)
        {
            return states.ToDictionary(state => state.Key, state => state.Evidence);
        }

        public static IEnumerable<double> Parse(this IEnumerable<State> states, IDistribution dist)
        {
            return states.Select(state =>
            {
                var max = double.IsPositiveInfinity(state.Max) ? state.Min * 2 : state.Max;
                return dist.Cdf(max) - dist.Cdf(state.Min);
            });
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

            var paramValues = VertexEvidence.ParseValues(str);

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
            foreach (var state in states) state.Evidence = belief;
        }

        public static void SetBelief(this IEnumerable<State> states, IEnumerable<double> beliefs)
        {
            states.SetProperty(beliefs, (state, belief) => state.Belief = belief);
        }

        public static void SetEvidence(this IEnumerable<State> states, string str)
        {
            var stateList = states as IList<State> ?? states.ToList();
            stateList.SetEvidence(stateList.Parse(str));
        }

        public static void SetEvidence(this IEnumerable<State> states, double evidence)
        {
            foreach (var state in states) state.Evidence = evidence;
        }

        public static void SetEvidence(this IEnumerable<State> states, IEnumerable<double> evidences)
        {
            states.SetProperty(evidences, (state, evidence) => state.Belief = evidence);
        }

        public static void SetProperty<TObject, TValue>(this IEnumerable<TObject> states, IEnumerable<TValue> values, Action<TObject, TValue> action)
        {
            var stateList = states as IList<TObject> ?? states.ToList();
            var valueList = values as IList<TValue> ?? values.ToList();

            if (stateList.Count() != valueList.Count()) throw new InvalidValueException("Number of states should be equal to the number of beliefs provided.");

            for (var i = 0; i < stateList.Count(); i++)
            {
                action(stateList[i], valueList[i]);
            }
        }
    }
}