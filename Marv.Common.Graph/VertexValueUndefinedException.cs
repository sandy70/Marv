using System;
using System.Runtime.Serialization;

namespace Marv.Graph
{
    [Serializable]
    public class VertexValueUndefindedException : Exception
    {
        public VertexValueUndefindedException()
        {
        }

        public VertexValueUndefindedException(string message)
            : base(message)
        {
        }

        public VertexValueUndefindedException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected VertexValueUndefindedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}