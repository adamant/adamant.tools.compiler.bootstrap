using System.Collections.Generic;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public abstract class GenericType : KnownType
    {
        [CanBeNull, ItemNotNull] public abstract IReadOnlyList<DataType> GenericParameterTypes { get; }
        public int? GenericArity => GenericParameterTypes?.Count;
        [CanBeNull, ItemCanBeNull] public abstract IReadOnlyList<DataType> GenericArguments { get; }
    }
}
