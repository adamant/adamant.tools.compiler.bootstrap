using System.Runtime.CompilerServices;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    public static class TypeOperations
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type<T> TypeOf<T>()
        {
            return new Type<T>();
        }
    }
}
