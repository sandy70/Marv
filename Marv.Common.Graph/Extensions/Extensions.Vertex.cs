using System;
using System.Linq;
using Marv.Common;

namespace Marv
{
    public static partial class Extensions
    {
        public static double Entropy(this NetworkNode node, double[] newValue, double[] oldValue = null)
        {
            CheckValueArrayLength(node, newValue);

            return newValue.Entropy();
        }

        public static double EntropyDifference(this NetworkNode node, double[] newValue, double[] oldValue = null)
        {
            CheckValueArrayLength(node, newValue);
            CheckValueArrayLength(node, oldValue);

            return newValue.Entropy() - oldValue.Entropy();
        }

        public static double Mean(this NetworkNode node, double[] newValue, double[] oldValue = null)
        {
            CheckVertexStatisticComputable(node, newValue);

            return node.States
                         .Select((state, i) => (newValue[i] * (state.Max + state.Min) / 2))
                         .Sum();
        }

        public static double MeanDifference(this NetworkNode node, double[] newValue, double[] oldValue = null)
        {
            CheckVertexStatisticComputable(node, newValue);
            CheckVertexStatisticComputable(node, oldValue);

            return node.Mean(newValue) - node.Mean(oldValue);
        }

        public static double StandardDeviation(this NetworkNode node, double[] newValue, double[] oldValue = null)
        {
            CheckVertexStatisticComputable(node, newValue);

            var mu = node.Mean(newValue);

            var stdev = node.States.Select((state, i) =>
                                             newValue[i] *
                                             (
                                                 1.0 / 3 * (Math.Pow(state.Max, 3) - Math.Pow(state.Min, 3)) +
                                                 Math.Pow(mu, 2) * (state.Max - state.Min) -
                                                 mu * (Math.Pow(state.Max, 2) - Math.Pow(state.Min, 2))
                                             ))
                              .Sum();

            return Math.Sqrt(stdev / node.States.Count);
        }

        private static void CheckVertexStatisticComputable(NetworkNode node, double[] value)
        {
            CheckValueArrayLength(node, value);

            if (node.Type != VertexType.Interval)
            {
                var message = String.Format("Mean is undefined for non-interval type node [{0}].", node);
                throw new InvalidValueException(message);
            }
        }

        private static void CheckValueArrayLength(NetworkNode node, double[] value)
        {
            if (node.States.Count != value.Length)
            {
                var message = String.Format("The length of value array [{0}] should be = number of states in this node [{1}:{2}].", value.Length, node, node.States.Count);
                throw new InvalidValueException(message);
            }
        }
    }
}