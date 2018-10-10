using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Types
{
    // A function type is the type of a declared function, method or constructor.
    // A function type may be generic and have generic parameters
    public class FunctionType : DataType
    {
        [NotNull] [ItemNotNull] public readonly IReadOnlyList<DataType> ParameterTypes;
        [NotNull] public readonly DataType ReturnType;

        public FunctionType([NotNull][ItemNotNull] IEnumerable<DataType> parameterTypes, [NotNull] DataType returnType)
        {
            Requires.NotNull(nameof(parameterTypes), parameterTypes);
            Requires.NotNull(nameof(returnType), returnType);
            ParameterTypes = parameterTypes.ToList().AsReadOnly().AssertNotNull();
            ReturnType = returnType;
        }

        public override string ToString()
        {
            return $"({string.Join(',', ParameterTypes)}) -> {ReturnType}";
        }
    }
}
