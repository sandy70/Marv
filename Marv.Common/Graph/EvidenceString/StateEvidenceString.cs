using System;
namespace Marv.Common
{
    public class StateEvidenceString : EvidenceString
    {
        public StateEvidenceString(string aString)
            : base(aString)
        {
        }

        public override IEvidence Parse(Vertex vertex)
        {
            IEvidence evidence = null;

            var stateIndex = -1;

            foreach (var state in vertex.States)
            {
                if (state.Key == this._string)
                {
                    stateIndex = vertex.States.IndexOf(state);
                }
            }

            if (stateIndex >= 0)
            {
                evidence = new HardEvidence
                {
                    StateIndex = stateIndex,
                    SynergiString = vertex.States[stateIndex].ValueString
                };
            }
            else
            {
                double value;

                if (Double.TryParse(this._string, out value))
                {
                    var evidenceArray = new double[vertex.States.Count];

                    foreach (var state in vertex.States)
                    {
                        if (state.Range.Bounds(value))
                        {
                            evidenceArray[vertex.States.IndexOf(state)] = 1;
                        }
                    }

                    evidence = new SoftEvidence
                    {
                        Evidence = evidenceArray,
                        SynergiString = this._string
                    };
                }
                else
                {
                    throw new Smile.SmileException("");
                }
            }
            return evidence;
        }
    }
}