using System;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public static class DataTypeExtensions
    {
        [NotNull]
        public static DataType AssertResolved([NotNull] this DataType type)
        {
            if (!type.IsResolved)
                throw new ArgumentException($"Type {type} not resolved");

            return type;
        }
    }
}
