using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    public abstract class GenericType : DataType
    {
        [CanBeNull, ItemNotNull] public abstract FixedList<DataType> GenericParameterTypes { get; }
        public int? GenericArity => GenericParameterTypes?.Count;
        [CanBeNull, ItemCanBeNull] public abstract FixedList<DataType> GenericArguments { get; }
    }
}
