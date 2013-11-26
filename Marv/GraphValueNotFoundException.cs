using System;

namespace Marv
{
    [Serializable]
    public class GraphValueNotFoundException : Exception
    {
        public GraphValueNotFoundException()
        {
        }

        public GraphValueNotFoundException(string message)
            : base(message)
        {
        }

        public GraphValueNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected GraphValueNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}