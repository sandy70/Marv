using System;

namespace Marv.Common
{
    [Serializable]
    public class InconsistentEvidenceException : Exception
    {
        public InconsistentEvidenceException()
        {
        }

        public InconsistentEvidenceException(string message)
            : base(message)
        {
        }

        public InconsistentEvidenceException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected InconsistentEvidenceException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}