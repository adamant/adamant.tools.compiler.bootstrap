using System;
using System.Runtime.Serialization;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    [Serializable]
    public class NonExhaustiveMatchException : Exception
    {
        private NonExhaustiveMatchException(string message)
            : base(message)
        {
        }

        protected NonExhaustiveMatchException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public static NonExhaustiveMatchException For<T>(T value)
            where T : class?
        {
            return new NonExhaustiveMatchException($"Matching value of type {value?.GetType().FullName ?? "<null>"}");
        }
    }
}
