using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public class GenericFunctionType : GenericType
    {
        [NotNull, ItemNotNull] public override IReadOnlyList<DataType> GenericParameterTypes { get; }
        [NotNull, ItemCanBeNull] public override IReadOnlyList<DataType> GenericArguments { get; }
        [NotNull] public readonly DataType ReturnType;

        public GenericFunctionType(
            [NotNull, ItemNotNull] IEnumerable<DataType> genericParameterTypes,
            [CanBeNull, ItemCanBeNull] IEnumerable<DataType> genericArguments,
            [NotNull] DataType returnType)
        {
            Requires.NotNull(nameof(genericParameterTypes), genericParameterTypes);
            Requires.NotNull(nameof(returnType), returnType);
            ReturnType = returnType;
            var genericParameterTypesList = genericParameterTypes.ToReadOnlyList();
            GenericParameterTypes = genericParameterTypesList;
            var genericArgumentsList = (genericArguments ?? genericParameterTypesList.Select(t => default(DataType))).ToReadOnlyList();
            Requires.That(nameof(genericArguments), genericArgumentsList.Count == genericParameterTypesList.Count);
            GenericArguments = genericArgumentsList;
        }

        public override string ToString()
        {
            return $"[{string.Join(',', GenericParameterTypes)}] -> {ReturnType}";
        }
    }
}
