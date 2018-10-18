using System;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    /// <summary>
    /// This creates a way to pass generic types as regular arguments
    /// </summary>
    public struct Type<T>
    {
        public static implicit operator Type(Type<T> _)
        {
            return typeof(T);
        }
    }
}
