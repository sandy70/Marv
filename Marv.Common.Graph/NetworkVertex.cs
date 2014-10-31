using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Marv
{
    public class NetworkVertex : IKeyed
    {
        public readonly List<NetworkVertex> Children = new List<NetworkVertex>();
        public readonly Dictionary<string, string> Properties = new Dictionary<string, string>();

        private ObservableCollection<State> states;
        private VertexType? type;

        public double[] InitialBelief
        {
            get { return this.States.Select(state => state.InitialBelief).ToArray(); }

            set
            {
                for (var i = 0; i < this.States.Count; i++)
                {
                    this.States[i].InitialBelief = value[i];
                }
            }
        }

        public string Key { get; set; }

        public ObservableCollection<State> States
        {
            get { return this.states ?? (this.states = this.ParseStates()); }
        }

        public VertexType Type
        {
            get { return this.type ?? (this.type = this.ParseSubType()).Value; }

            set { this.type = value; }
        }

        public ObservableCollection<string> ParseGroups()
        {
            var groups = new ObservableCollection<string>();

            if (this.Properties.ContainsKey("groups"))
            {
                var groupsValueString = this.Properties["groups"];

                var parts = groupsValueString.Split(new[]
                {
                    '"', ','
                }, StringSplitOptions.RemoveEmptyEntries);

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
                try
                {
                    return this.Properties[propertyName].Dequote().ParseJson<T>();
                }
                catch (Exception)
                {
                    return new T();
                }
            }
            return new T();
        }

        public Point ParsePosition()
        {
            var posValueString = this.Properties["position"];

            var parts = posValueString.Split(new[]
            {
                '(', ')', ' '
            }, StringSplitOptions.RemoveEmptyEntries);

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

        public string ParseStringProperty(string str)
        {
            var htmlDesc = "";

            if (this.Properties.ContainsKey(str))
            {
                htmlDesc = this.Properties[str].Trim().Dequote();
            }

            return htmlDesc;
        }

        private ObservableCollection<State> ParseStates()
        {
            var theStates = new ObservableCollection<State>();
            string subtype;

            if (this.Properties.TryGetValue("subtype", out subtype))
            {
                if (subtype.Equals("interval"))
                {
                    var stateStrings = this.Properties["state_values"]
                        .Split("() ".ToArray(), StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

                    var nStatesStrings = stateStrings.Count;

                    for (var i = 0; i < nStatesStrings - 1; i++)
                    {
                        theStates.Add(new State
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
                        .Split("() ".ToArray(), StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

                    var nStatesStrings = stateStrings.Count;

                    for (var i = 0; i < nStatesStrings; i++)
                    {
                        var value = double.Parse(stateStrings[i]);

                        theStates.Add(new State
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

                                theStates.Add(new State
                                {
                                    Key = stateString,
                                    Max = range == null ? 0 : range.Max,
                                    Min = range == null ? 0 : range.Min,
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

            return theStates;
        }

        private VertexType ParseSubType()
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