using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    // A function type is the type of a declared function, method or constructor.
    // A function type may be generic and have generic parameters
    public class FunctionType : KnownType
    {
        [NotNull] [ItemNotNull] public readonly IReadOnlyList<KnownType> ParameterTypes;
        [NotNull] public readonly KnownType ReturnType;

        public FunctionType([NotNull][ItemNotNull] IEnumerable<KnownType> parameterTypes, [NotNull] KnownType returnType)
        {
            Requires.NotNull(nameof(parameterTypes), parameterTypes);
            Requires.NotNull(nameof(returnType), returnType);
            ParameterTypes = parameterTypes.ToReadOnlyList();
            ReturnType = returnType;
        }

        public override string ToString()
        {
            return $"({string.Join(',', ParameterTypes)}) -> {ReturnType}";
        }
    }
}
