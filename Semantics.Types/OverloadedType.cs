using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public class OverloadedType : UnresolvedType
    {
        [NotNull, ItemNotNull] public readonly IReadOnlyList<DataType> Types;

        public OverloadedType([NotNull, ItemNotNull] IEnumerable<DataType> types)
        {
            Requires.NotNull(nameof(types), types);
            Types = types.AssertItemNotNull().ToReadOnlyList();
        }

        [NotNull]
        public override string ToString()
        {
            return $"⧼{string.Join('|', Types)}⧽";
        }
    }
}
