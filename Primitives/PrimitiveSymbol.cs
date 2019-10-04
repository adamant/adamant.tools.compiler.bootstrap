using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Primitives
{
    internal abstract class PrimitiveSymbol : ISymbol
    {
        public Name FullName { get; }
        public SymbolSet ChildSymbols { get; protected set; } = SymbolSet.Empty;

        protected PrimitiveSymbol(Name fullName)
        {
            FullName = fullName;
        }
    }
}
