using System;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public static class DataTypeExtensions
    {
        [NotNull]
        public static KnownType AssertKnown([CanBeNull] this DataType type)
        {
            type.AssertNotNull();
            if (type is KnownType knownType)
                return knownType;

            throw new InvalidOperationException("Data type should be known");
        }
    }
}
