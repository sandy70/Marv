using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Marv.Common
{
    public class NodeEvidence
    {
        public double[] Params { get; set; }
        public string StateKey { get; set; }

        [JsonConverter(typeof (StringEnumConverter))]
        public NodeEvidenceType Type { get; set; }

        public double[] Value { get; set; }

        public static List<double> ParseEvidenceParams(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }

            var values = new List<double>();

            var parts = str.Trim().Split("(),: ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                double value;
                if (double.TryParse(part, out value))
                {
                    values.Add(value);
                }
            }

            return values;
        }

        public override string ToString()
        {
            if (this.Params == null)
            {
                return null;
            }

            if (this.Type == NodeEvidenceType.Distribution)
            {
                return this.Params.String("{0:F2}");
            }

            if (this.Type == NodeEvidenceType.Normal)
            {
                return "NORM" + this.Params.String().Enquote('(', ')');
            }

            if (this.Type == NodeEvidenceType.Number)
            {
                return this.Params[0].ToString();
            }

            if (this.Type == NodeEvidenceType.Range)
            {
                return this.Params[0] + ":" + this.Params[1];
            }

            if (this.Type == NodeEvidenceType.State)
            {
                return this.StateKey;
            }

            if (this.Type == NodeEvidenceType.Triangular)
            {
                return "TRI" + this.Params.String("{0:F2}").Enquote('(', ')');
            }

            return null;
        }
    }
}