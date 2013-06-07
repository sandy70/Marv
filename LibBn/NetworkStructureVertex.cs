using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace LibBn
{
    public class NetworkStructureVertex
    {
        public List<NetworkStructureVertex> Children = new List<NetworkStructureVertex>();
        public string Key = "";
        public Dictionary<string, string> Properties = new Dictionary<string, string>();

        public ObservableCollection<string> ParseGroups()
        {
            var groups = new ObservableCollection<string>();

            if (this.Properties.ContainsKey("group"))
            {
                var groupsValueString = this.Properties["group"];

                var parts = groupsValueString.Split(new char[] { '"', ',' }, StringSplitOptions.RemoveEmptyEntries);

                groups = new ObservableCollection<string>(parts);

                if (!groups.Contains("all"))
                {
                    groups.Add("all");
                }
            }
            else
            {
                groups.Add("all");
            }

            return groups;
        }

        public Point ParsePosition()
        {
            var posValueString = this.Properties["position"];

            var parts = posValueString.Split(new char[] { '(', ')', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var position = new Point
            {
                X = double.Parse(parts[0]),
                Y = double.Parse(parts[1])
            };

            return position;
        }

        public Dictionary<string, Point> ParsePositionByGroup()
        {
            var positionsByGroup = new Dictionary<string, Point>();

            if (this.Properties.ContainsKey("grouppositions"))
            {
                var valueString = this.Properties["grouppositions"];

                string[] parts = valueString.Split(new char[] { ',', '"' }, StringSplitOptions.RemoveEmptyEntries);

                int nPositions = parts.Count() / 3;

                for (int i = 0; i < nPositions; i++)
                {
                    double x; double y;
                    string group = parts[3 * i];

                    if (double.TryParse(parts[3 * i + 1], out x) && double.TryParse(parts[3 * i + 2], out y))
                    {
                        positionsByGroup.Add(group, new System.Windows.Point(x, y));
                    }
                }
            }

            return positionsByGroup;
        }

        public List<string> ParseStates()
        {
            var states = new List<string>();
            var subtype = "";

            if (this.Properties.TryGetValue("subtype", out subtype))
            {
                if (subtype.Equals("interval"))
                {
                    var stateStrings = this.Properties["state_values"]
                                           .Split(new char[] { '(', ')', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                           .ToList();

                    var nStatesStrings = stateStrings.Count;

                    for (int i = 0; i < nStatesStrings - 1; i++)
                    {
                        states.Add(String.Format("{0} - {1}", stateStrings[i], stateStrings[i + 1]));
                    }
                }
                else if (subtype.Equals("number"))
                {
                    var stateStrings = this.Properties["state_values"]
                                           .Split(new char[] { '(', ')', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                           .ToList();

                    var nStatesStrings = stateStrings.Count;

                    for (int i = 0; i < nStatesStrings; i++)
                    {
                        states.Add(stateStrings[i]);
                    }
                }
            }
            else
            {
                var statesString = "";
                var stateString = "";

                if (this.Properties.TryGetValue("states", out statesString))
                {
                    bool isReading = false;
                    foreach (var c in statesString.ToCharArray())
                    {
                        if (c == '"')
                        {
                            if (isReading)
                            {
                                states.Add(stateString);
                                stateString = "";
                            }

                            isReading = !isReading;
                            continue;
                        }

                        if (isReading)
                        {
                            stateString += c;
                        }
                    }
                }
                else
                {
                    // something is really really wrong. We could flag errors here.
                }
            }

            return states;
        }

        public void ParseStatesMinMax(ObservableCollection<State> states)
        {
            var subtype = "";

            if (this.Properties.TryGetValue("subtype", out subtype))
            {
                if (subtype.Equals("interval"))
                {
                    var stateStrings = this.Properties["state_values"]
                                           .Split(new char[] { '(', ')', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                           .ToList();

                    var nStatesStrings = stateStrings.Count;

                    for (int i = 0; i < nStatesStrings - 1; i++)
                    {
                        states[i].Min = double.Parse(stateStrings[i]);
                        states[i].Max = double.Parse(stateStrings[i + 1]);
                    }
                }
                else if (subtype.Equals("number"))
                {
                    var stateStrings = this.Properties["state_values"]
                                           .Split(new char[] { '(', ')', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                           .ToList();

                    var nStatesStrings = stateStrings.Count;

                    for (int i = 0; i < nStatesStrings; i++)
                    {
                        states[i].Min = double.Parse(stateStrings[i]);
                        states[i].Max = double.Parse(stateStrings[i]);
                    }
                }
            }
        }

        public string ParseStringProperty(string str)
        {
            var htmlDesc = "";

            if (this.Properties.ContainsKey(str))
            {
                htmlDesc = this.Properties[str].Split("\"".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            }

            return htmlDesc;
        }

        public VertexType ParseSubType()
        {
            var typeString = this.ParseStringProperty("subtype");

            if (typeString.Equals("number"))
            {
                return VertexType.Number;
            }
            else if (typeString.Equals("interval"))
            {
                return VertexType.Interval;
            }
            else
            {
                return VertexType.None;
            }
        }
    }
}