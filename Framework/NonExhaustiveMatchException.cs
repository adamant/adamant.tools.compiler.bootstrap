using System;
using System.Runtime.Serialization;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    [Serializable]
    internal class NonExhaustiveMatchException : Exception
    {
        public NonExhaustiveMatchException()
        {
        }

        public NonExhaustiveMatchException(string message)
            : base(message)
        {
        }

        public NonExhaustiveMatchException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected NonExhaustiveMatchException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
