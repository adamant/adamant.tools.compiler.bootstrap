using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public class OverloadedType : DataType
    {
        [NotNull, ItemNotNull] public readonly IReadOnlyList<DataType> Types;
        public override bool IsResolved { get; }

        public OverloadedType([NotNull, ItemNotNull] IEnumerable<DataType> types)
        {
            Requires.NotNull(nameof(types), types);
            Types = types.AssertItemNotNull().ToReadOnlyList();
            IsResolved = Types.All(t => t.IsResolved);
        }

        [NotNull]
        public override string ToString()
        {
            return $"⧼{string.Join('|', Types)}⧽";
        }
    }
}
