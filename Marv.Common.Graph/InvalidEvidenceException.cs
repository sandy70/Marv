using System;
using System.Runtime.Serialization;

namespace Marv.Common.Graph
{
    [Serializable]
    public class InvalidEvidenceException : Exception
    {
        public string VertexKey { get; set; }

        public InvalidEvidenceException() {}
        public InvalidEvidenceException(string message) : base(message) {}
        public InvalidEvidenceException(string message, Exception inner) : base(message, inner) {}

        protected InvalidEvidenceException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) {}
    }
}