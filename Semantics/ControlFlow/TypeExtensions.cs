using System;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow
{
    internal static class TypeExtensions
    {
        public static DataType Fulfilled(this DataType? type)
        {
            return type ?? throw new InvalidOperationException("Type not fulfilled");
        }
    }
}
