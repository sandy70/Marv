using System;
using System.Collections.Generic;
using System.Linq;

namespace LibBn
{
    public class NetworkStructureNode
    {
        public string Key = "";
        public Dictionary<string, string> Properties = new Dictionary<string, string>();

        internal List<string> ParseStates()
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
            }
            else
            {
                var statesString = "";
                var stateString = "";

                if(this.Properties.TryGetValue("states", out statesString))
                {
                    bool isReading = false;
                    foreach(var c in statesString.ToCharArray())
                    {
                        if(c == '"')
                        {
                            if(isReading)
                            {
                                states.Add(stateString);
                                stateString = "";
                            }

                            isReading = !isReading;
                            continue;
                        }

                        if(isReading)
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
    }
}