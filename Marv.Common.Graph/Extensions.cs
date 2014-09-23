using System;
using System.Collections.Generic;
using System.Linq;

namespace Marv
{
    public static partial class Extensions
    {
        public static void AddUnique(this ICollection<Edge> edges, Vertex source, Vertex target, EdgeConnectorPositions connectorPostions = null)
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

        public static Dictionary<string, double[]> GetBeliefs(this Smile.Network network)
        {
            var beliefs = new Dictionary<string, double[]>();

            foreach (var vertexKey in network.GetAllNodeIds())
            {
                beliefs[vertexKey] = network.GetNodeValue(vertexKey);
            }

            return beliefs;
        }

        public static Dict<string, VertexData> GetData(this Smile.Network network)
        {
            var graphData = new Dict<string, VertexData>();

            foreach (var vertexKey in network.GetAllNodeIds())
            {
                graphData[vertexKey].Belief = network.GetNodeValue(vertexKey);
                graphData[vertexKey].Evidence = network.GetSoftEvidence(vertexKey);
            }

            return graphData;
        }

        public static void SetProperty<TObject, TValue>(this IEnumerable<TObject> objects, IEnumerable<TValue> values, Action<TObject, TValue> action)
        {
            var stateList = objects as IList<TObject> ?? objects.ToList();
            var valueList = values as IList<TValue> ?? values.ToList();

            if (stateList.Count() != valueList.Count())
            {
                throw new InvalidValueException("Number of objects should be equal to the number of values provided.");
            }

            for (var i = 0; i < stateList.Count(); i++)
            {
                action(stateList[i], valueList[i]);
            }
        }
    }
}