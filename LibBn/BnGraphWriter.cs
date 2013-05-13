using Smile;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibBn
{
    public static class BnGraphWriter
    {
        public static void WritePositions(string originalFileName, BnGraph graph)
        {
            BnGraphWriter.WritePositions(originalFileName, originalFileName, graph);
        }

        public static void WritePositions(string originalFileName, string outputFileName, BnGraph graph)
        {
            NetworkHeaderReader networkHeaderReader = new NetworkHeaderReader();
            List<string> headerLines = networkHeaderReader.Read(originalFileName);

            Network network = new Network();
            network.ReadFile(originalFileName);

            foreach (var nodeHandle in network.GetAllNodes())
            {
                UserProperty[] originalProperties = network.GetNodeUserProperties(nodeHandle);
                List<UserProperty> newProperties = new List<UserProperty>();

                string key = network.GetNodeId(nodeHandle);
                BnVertex vertex = graph.GetVertexByKey(key);

                // Add group positions
                BnGraphWriter.AddGroupPositions(newProperties, vertex);

                // Add groups
                BnGraphWriter.AddGroups(newProperties, vertex);

                // Add all the original properties except for the modified ones.
                foreach (var property in originalProperties)
                {
                    if (property.name.Equals("grouppositions") ||
                        property.name.Equals("group"))
                    {
                        // pass
                    }
                    else
                    {
                        newProperties.Add(new UserProperty { name = property.name, value = property.value });
                    }
                }

                network.SetNodeUserProperties(nodeHandle, newProperties.ToArray());
            }

            network.WriteFile(outputFileName);

            NetworkHeaderWriter networkHeaderWriter = new NetworkHeaderWriter();
            networkHeaderWriter.Write(outputFileName, headerLines);
        }

        private static void AddGroupPositions(List<UserProperty> newProperties, BnVertex vertex)
        {
            string grouppositions = "";

            if (vertex.PositionsByGroup.Count > 0)
            {
                foreach (var kvp in vertex.PositionsByGroup.AllButLast())
                {
                    grouppositions += kvp.Key + "," + kvp.Value.X + "," + kvp.Value.Y + ",";
                }

                var kvpLast = vertex.PositionsByGroup.Last();
                grouppositions += kvpLast.Key + "," + kvpLast.Value.X + "," + kvpLast.Value.Y;
            }

            newProperties.Add(new UserProperty { name = "grouppositions", value = grouppositions });
        }

        private static void AddGroups(List<UserProperty> newProperties, BnVertex vertex)
        {
            string groupString = "";

            if (vertex.Groups.Count > 0)
            {
                foreach (var group in vertex.Groups.AllButLast())
                {
                    groupString += group + ",";
                }

                groupString += vertex.Groups.Last();
            }

            newProperties.Add(new UserProperty { name = "group", value = groupString });
        }
    }
}