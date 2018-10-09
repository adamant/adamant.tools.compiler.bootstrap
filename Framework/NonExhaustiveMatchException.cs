using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

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

        [NotNull]
        public static NonExhaustiveMatchException ForEnum<T>(T value)
            where T : struct, Enum
        {
            return new NonExhaustiveMatchException($"Matching value {typeof(T).Name}.{value}");
        }

        [NotNull]
        public static NonExhaustiveMatchException For([CanBeNull] object value)
        {
            return new NonExhaustiveMatchException($"Matching value of type {value?.GetType()?.FullName}");
        }
    }
}
