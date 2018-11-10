using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    /// <summary>
    /// A function type is the type of a declared function, method or constructor.
    /// If a function is also a generic function, its type will be a
    /// `MetaFunctionType` whose result is a function type.
    /// </summary>
    public class FunctionType : DataType
    {
        [NotNull, ItemNotNull] public readonly IReadOnlyList<DataType> ParameterTypes;
        [NotNull] public readonly DataType ResultType;
        public override bool IsResolved { get; }

        public FunctionType(
            [NotNull, ItemNotNull] IEnumerable<DataType> parameterTypes,
            [NotNull] DataType resultType)
        {
            Requires.NotNull(nameof(parameterTypes), parameterTypes);
            Requires.NotNull(nameof(resultType), resultType);
            ParameterTypes = parameterTypes.ItemsNotNull().ToReadOnlyList();
            ResultType = resultType;
            IsResolved = ParameterTypes.All(pt => pt.IsResolved) && ResultType.IsResolved;
        }

        public override string ToString()
        {
            return $"({string.Join(',', ParameterTypes)}) -> {ResultType}";
        }
    }
}
