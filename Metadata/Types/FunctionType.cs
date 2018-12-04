using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    /// <summary>
    /// A function type is the type of a declared function, method or constructor.
    /// If a function is also a generic function, its type will be a
    /// `MetaFunctionType` whose result is a function type.
    /// </summary>
    public class FunctionType : ReferenceType
    {
        [NotNull, ItemNotNull] public readonly FixedList<DataType> ParameterTypes;
        public int Arity => ParameterTypes.Count;
        [NotNull] public readonly DataType ReturnType;
        public override bool IsResolved { get; }

        public FunctionType(
            [NotNull, ItemNotNull] IEnumerable<DataType> parameterTypes,
            [NotNull] DataType returnType)
        {
            ParameterTypes = parameterTypes.ToFixedList();
            ReturnType = returnType;
            IsResolved = ParameterTypes.All(pt => pt.IsResolved) && ReturnType.IsResolved;
        }

        public override string ToString()
        {
            return $"({string.Join(", ", ParameterTypes)}) -> {ReturnType}";
        }
    }
}
