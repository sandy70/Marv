using Smile;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace LibBn
{
    public class BnGraphReader<TVertex>
        where TVertex : BnVertex, new()
    {
        public static BnGraph Read(string fileName)
        {
            BnGraph graph = new BnGraph();

            var network = graph.Network;
            network.ReadFile(fileName);
            network.UpdateBeliefs();

            ObservableCollection<TVertex> vertices = BnGraphReader<TVertex>.GetVertices(network);

            graph.AddVertices(vertices);

            foreach (var nodeHandle in network.GetAllNodes())
            {
                int[] childHandles = network.GetChildren(nodeHandle);

                foreach (var childHandle in childHandles)
                {
                    string key1 = network.GetNodeId(nodeHandle);
                    string key2 = network.GetNodeId(childHandle);

                    graph.AddEdge(key1, key2);
                }
            }

            return graph;
        }

        public static async Task<BnGraph> ReadAsync(string fileName)
        {
            return await Task.Run(() => BnGraphReader<TVertex>.Read(fileName));
        }

        private static ObservableCollection<TVertex> GetVertices(Network network)
        {
            ObservableCollection<TVertex> vertices = new ObservableCollection<TVertex>();

            foreach (var nodeHandle in network.GetAllNodes())
            {
                TVertex vertex = new TVertex();
                ObservableCollection<BnState> states = new ObservableCollection<BnState>();

                int nStates = network.GetOutcomeCount(nodeHandle);
                double[] values = network.GetNodeValue(nodeHandle);

                UserProperty[] props = network.GetNodeUserProperties(nodeHandle);
                string[] names = new string[nStates];

                // Parse position
                Rectangle position = network.GetNodePosition(nodeHandle);
                vertex.Position = new System.Windows.Point(position.X, position.Y);

                foreach (var p in props)
                {
                    // Parse state
                    if (p.name.Contains("HR_State_"))
                    {
                        int stateIndex;
                        int.TryParse(p.name.Substring(9), out stateIndex);

                        if (stateIndex < nStates)
                        {
                            names[stateIndex] = p.value;
                        }
                    }

                    // Parse Description
                    else if (p.name.Equals("HR_HTML_Desc"))
                    {
                        vertex.Description = p.value;
                    }

                    // Parse groups
                    else if (p.name.Equals("group"))
                    {
                        string[] parts = p.value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var part in parts)
                        {
                            vertex.Groups.Add(part);
                        }

                        if (!vertex.Groups.Contains(Groups.All))
                        {
                            vertex.Groups.Add(Groups.All);
                        }
                    }

                    // Parse group positions
                    else if (p.name.Equals("grouppositions"))
                    {
                        string[] parts = p.value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        int nPositions = parts.Count() / 3;

                        for (int i = 0; i < nPositions; i++)
                        {
                            double x; double y;
                            string group = parts[3 * i];
                            
                            if (double.TryParse(parts[3 * i + 1], out x) && double.TryParse(parts[3 * i + 2], out y))
                            {
                                vertex.PositionsByGroup.Add(group, new System.Windows.Point(x, y));
                            }
                        }
                    }

                    // Parse headers
                    else if (p.name.Equals("header"))
                    {
                        if (p.value.Contains("t"))
                        {
                            vertex.IsHeader = true;
                        }
                        else
                        {
                            vertex.IsHeader = false;
                        }
                    }

                    // Parse Header of Group
                    else if (p.name.Equals("headerofgroup"))
                    {
                        vertex.HeaderOfGroup = p.value;

                        //int group;

                        //if (int.TryParse(p.value, out group))
                        //{
                        //    vertex.HeaderOfGroup = group;
                        //}
                    }

                    // parse units
                    else if (p.name.Equals("unit"))
                    {
                        vertex.Units = p.value;
                    }
                }

                for (int s = 0; s < nStates; s++)
                {
                    states.Add(new BnState
                    {
                        Key = names[s],
                        Value = values[s]
                    });
                }

                vertex.Key = network.GetNodeId(nodeHandle);
                vertex.Name = network.GetNodeName(nodeHandle);
                vertex.States = states;

                vertices.Add(vertex);
            }

            return vertices;
        }
    }
}