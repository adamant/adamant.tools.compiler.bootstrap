using System;
using System.Runtime.Serialization;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    [Serializable]
    public class NonExhaustiveMatchException : Exception
    {
        internal NonExhaustiveMatchException()
        {
        }

        internal NonExhaustiveMatchException(string message)
            : base(message)
        {
        }

        internal NonExhaustiveMatchException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected NonExhaustiveMatchException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public static NonExhaustiveMatchException ForEnum<T>(T value)
            where T : struct, Enum
        {
            return new NonExhaustiveMatchException($"Matching value {typeof(T).Name}.{value}");
        }

        public static NonExhaustiveMatchException For(object value)
        {
            return new NonExhaustiveMatchException($"Matching value of type {value?.GetType().FullName}");
        }
    }
}
