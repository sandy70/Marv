using System;
using System.Runtime.Serialization;

namespace Marv
{
    [Serializable]
    public class InvalidValueException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public InvalidValueException() {}
        public InvalidValueException(string message) : base(message) {}
        public InvalidValueException(string message, Exception inner) : base(message, inner) {}

        protected InvalidValueException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}