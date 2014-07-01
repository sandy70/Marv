﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Marv.Common.Graph
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

                var parts = groupsValueString.Split(new[] {'"', ','}, StringSplitOptions.RemoveEmptyEntries);

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
            return false;
        }

        public T ParseJson<T>(string propertyName) where T : new()
        {
            if (this.Properties.ContainsKey(propertyName))
            {
                return this.Properties[propertyName].Dequote().ParseJson<T>();
            }
            return new T();
        }

        public Point ParsePosition()
        {
            var posValueString = this.Properties["position"];

            var parts = posValueString.Split(new[] {'(', ')', ' '}, StringSplitOptions.RemoveEmptyEntries);

            var position = new Point
            {
                X = double.Parse(parts[0]),
                Y = double.Parse(parts[1])
            };

            return position;
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

        public ModelCollection<State> ParseStates()
        {
            var states = new ModelCollection<State>();
            string subtype;

            if (this.Properties.TryGetValue("subtype", out subtype))
            {
                if (subtype.Equals("interval"))
                {
                    var stateStrings = this.Properties["state_values"]
                        .Split(new[] {'(', ')', ' '}, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

                    var nStatesStrings = stateStrings.Count;

                    for (var i = 0; i < nStatesStrings - 1; i++)
                    {
                        states.Add(new State
                        {
                            Key = String.Format("{0} - {1}", stateStrings[i], stateStrings[i + 1]),

                            // we use ParseDouble to take care of infinities
                            Max = stateStrings[i + 1].ParseDouble(),
                            Min = stateStrings[i].ParseDouble()
                        });
                    }
                }
                else if (subtype.Equals("number"))
                {
                    var stateStrings = this.Properties["state_values"]
                        .Split(new[] {'(', ')', ' '}, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

                    var nStatesStrings = stateStrings.Count;

                    for (var i = 0; i < nStatesStrings; i++)
                    {
                        var value = double.Parse(stateStrings[i]);

                        states.Add(new State
                        {
                            Key = stateStrings[i],
                            Min = value,
                            Max = value
                        });
                    }
                }
            }
            else
            {
                string statesString;
                var stateString = "";
                var stateIndex = 0;

                if (this.Properties.TryGetValue("states", out statesString))
                {
                    var isReading = false;
                    foreach (var c in statesString)
                    {
                        if (c == '"')
                        {
                            if (isReading)
                            {
                                var range = this.ParseStateRange(stateIndex);
                                var max = 0.0;
                                var min = 0.0;

                                if (range != null)
                                {
                                    max = range.Max;
                                    min = range.Min;
                                }

                                states.Add(new State
                                {
                                    Key = stateString,
                                    Max = max,
                                    Min = min
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
                return VertexType.Numbered;
            }
            if (typeString.Equals("interval"))
            {
                return VertexType.Interval;
            }
            return VertexType.Labelled;
        }
    }
}