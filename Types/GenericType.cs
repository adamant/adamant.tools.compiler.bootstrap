using System.Collections.Generic;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    public abstract class GenericType : DataType
    {
        [CanBeNull, ItemNotNull] public abstract IReadOnlyList<DataType> GenericParameterTypes { get; }
        public int? GenericArity => GenericParameterTypes?.Count;
        [CanBeNull, ItemCanBeNull] public abstract IReadOnlyList<DataType> GenericArguments { get; }
    }
}
