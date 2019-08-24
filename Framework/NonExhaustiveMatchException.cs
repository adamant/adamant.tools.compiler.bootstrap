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
            where T : struct, Enum
        {
            return new NonExhaustiveMatchException($"Matching value {typeof(T).Name}.{value}");
        }

        public static NonExhaustiveMatchException For(object value)
        {
            return new NonExhaustiveMatchException($"Matching value of type {value?.GetType()?.FullName ?? "<null>"}");
        }
    }
}
