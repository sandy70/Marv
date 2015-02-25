using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Marv.Common
{
    public class VertexEvidence
    {
        public double[] Params { get; set; }
        public string StateKey { get; set; }

        [JsonConverter(typeof (StringEnumConverter))]
        public VertexEvidenceType Type { get; set; }

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

            if (this.Type == VertexEvidenceType.Distribution)
            {
                return this.Params.String("{0:F2}");
            }

            if (this.Type == VertexEvidenceType.Normal)
            {
                return "NORM" + this.Params.String().Enquote('(', ')');
            }

            if (this.Type == VertexEvidenceType.Number)
            {
                return this.Params[0].ToString();
            }

            if (this.Type == VertexEvidenceType.Range)
            {
                return this.Params[0] + ":" + this.Params[1];
            }

            if (this.Type == VertexEvidenceType.State)
            {
                return this.StateKey;
            }

            if (this.Type == VertexEvidenceType.Triangular)
            {
                return "TRI" + this.Params.String("{0:F2}").Enquote('(', ')');
            }

            return null;
        }
    }
}