using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Primitives
{
    public class PrimitiveSymbol : ISymbol
    {
        public Name FullName { get; }
        [NotNull] public DataType Type { get; set; }
        [NotNull] public FixedDictionary<SimpleName, ISymbol> ChildSymbols { get; }

        public PrimitiveSymbol(
            [NotNull] Name fullName,
            [CanBeNull] DataType type,
            [CanBeNull, ItemNotNull] IEnumerable<ISymbol> childSymbols = null)
        {
            FullName = fullName;
            Type = type;
            ChildSymbols = childSymbols?.ToFixedDictionary(s => s.FullName.UnqualifiedName)
                           ?? FixedDictionary<SimpleName, ISymbol>.Empty;
        }
    }
}
