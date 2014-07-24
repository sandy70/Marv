﻿using System;
using System.Linq;

namespace Marv.Common.Graph
{
    public static partial class Extensions
    {
        public static double Entropy(this Vertex vertex, double[] newValue, double[] oldValue = null)
        {
            CheckVertexStatisticComputable(vertex, newValue);

            return newValue.Entropy();
        }

        public static double EntropyDifference(this Vertex vertex, double[] newValue, double[] oldValue = null)
        {
            CheckVertexStatisticComputable(vertex, newValue);
            CheckVertexStatisticComputable(vertex, oldValue);

            return newValue.Entropy() - oldValue.Entropy();
        }

        public static double Mean(this Vertex vertex, double[] newValue, double[] oldValue = null)
        {
            CheckVertexStatisticComputable(vertex, newValue);

            return vertex.States
                .Select((state, i) => (newValue[i] * (state.Max + state.Min) / 2))
                .Sum();
        }

        public static double MeanDifference(this Vertex vertex, double[] newValue, double[] oldValue = null)
        {
            CheckVertexStatisticComputable(vertex, newValue);
            CheckVertexStatisticComputable(vertex, oldValue);

            return vertex.Mean(newValue) - vertex.Mean(oldValue);
        }

        public static double StandardDeviation(this Vertex vertex, double[] newValue, double[] oldValue = null)
        {
            CheckVertexStatisticComputable(vertex, newValue);

            var mu = vertex.Mean(newValue);

            var stdev = vertex.States.Select((state, i) =>
                newValue[i] *
                (
                    1.0 / 3 * (Math.Pow(state.Max, 3) - Math.Pow(state.Min, 3)) +
                    Math.Pow(mu, 2) * (state.Max - state.Min) -
                    mu * (Math.Pow(state.Max, 2) - Math.Pow(state.Min, 2))
                    ))
                .Sum();

            return Math.Sqrt(stdev / vertex.States.Count);
        }

        private static void CheckVertexStatisticComputable(Vertex vertex, double[] value)
        {
            if (vertex.States.Count != value.Length)
            {
                var message = String.Format("The length of value array [{0}] should be = number of states in this vertex [{1}:{2}].", value.Length, vertex, vertex.States.Count);
                throw new InvalidValueException(message);
            }

            if (vertex.Type != VertexType.Interval)
            {
                var message = String.Format("Mean is undefined for non-interval type vertex [{0}].", vertex);
                throw new InvalidValueException(message);
            }
        }
    }
}