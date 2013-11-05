using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Marv.Common
{
    public class NetworkStructureVertex
    {
        public List<NetworkStructureVertex> Children = new List<NetworkStructureVertex>();
        public string Key = "";
        public Dictionary<string, string> Properties = new Dictionary<string, string>();

        public ObservableCollection<string> ParseGroups()
        {
            var groups = new ObservableCollection<string>();

            if (this.Properties.ContainsKey("groups"))
            {
                var groupsValueString = this.Properties["groups"];

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

        public bool ParseIsExpanded()
        {
            var valueString = this.ParseStringProperty("isexpanded");

            if (valueString.ToLower().Equals("true"))
            {
                return true;
            }
            else
            {
                return false;
            }
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

        public Sequence<double> ParseStateRange(int stateIndex)
        {
            var key = "HR_State_" + stateIndex;
            var range = new Sequence<double>();

            if (this.Properties.ContainsKey(key))
            {
                var stateString = this.Properties[key];

                if (String.IsNullOrEmpty(stateString))
                {
                    range = null;
                }
                else
                {
                    var stateStringParts = stateString.Split("\":".ToArray(), StringSplitOptions.RemoveEmptyEntries);

                    if (stateStringParts.Count() == 2)
                    {
                        range.Add(Double.Parse(stateStringParts[1]));
                        range.Add(Double.Parse(stateStringParts[0]));
                    }
                    else
                    {
                        range = null;
                    }
                }
            }
            else
            {
                range = null;
            }

            return range;
        }

        public ObservableCollection<State> ParseStates()
        {
            var states = new ObservableCollection<State>();
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
                        states.Add(new State
                        {
                            Key = String.Format("{0} - {1}", stateStrings[i], stateStrings[i + 1]),

                            Range = new Sequence<double>
                            {
                               double.Parse(stateStrings[i + 1]),
                               double.Parse(stateStrings[i])
                            }
                        });
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
                        states.Add(new State
                        {
                            Key = stateStrings[i],

                            Range = new Sequence<double>
                            {
                                double.Parse(stateStrings[i])
                            }
                        });
                    }
                }
            }
            else
            {
                var statesString = "";
                var stateString = "";
                var stateIndex = 0;

                if (this.Properties.TryGetValue("states", out statesString))
                {
                    bool isReading = false;
                    foreach (var c in statesString.ToCharArray())
                    {
                        if (c == '"')
                        {
                            if (isReading)
                            {
                                var range = this.ParseStateRange(stateIndex);

                                states.Add(new State
                                {
                                    Key = stateString,
                                    Range = range
                                });

                                stateIndex++;
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