using System;

namespace Marv
{
    [Serializable]
    public class ModelValueNotFoundException : Exception
    {
        public ModelValueNotFoundException()
        {
        }

        public ModelValueNotFoundException(string message)
            : base(message)
        {
        }

        public ModelValueNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected ModelValueNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}