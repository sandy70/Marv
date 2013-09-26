﻿using System;

namespace LibPipeline
{
    [Serializable]
    public class OdbDataNotFoundException : Exception
    {
        public OdbDataNotFoundException()
        {
        }

        public OdbDataNotFoundException(string message)
            : base(message)
        {
        }

        public OdbDataNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected OdbDataNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}