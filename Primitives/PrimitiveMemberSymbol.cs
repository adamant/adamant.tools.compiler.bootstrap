using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Primitives
{
    public class PrimitiveMemberSymbol : ISymbol
    {
        public Name FullName { get; }
        [NotNull] private readonly FunctionType type;
        [NotNull] DataType ISymbol.Type => type;
        [NotNull] public FixedDictionary<SimpleName, ISymbol> ChildSymbols => FixedDictionary<SimpleName, ISymbol>.Empty;

        public PrimitiveMemberSymbol([NotNull] string name, [NotNull] FunctionType type)
        {
            FullName = new SimpleName(name);
            this.type = type;
        }
    }
}
