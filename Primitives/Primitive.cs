using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Primitives
{
    internal abstract class Primitive : ISymbol
    {
        public Name FullName { get; }
        public SymbolSet ChildSymbols { get; protected set; } = SymbolSet.Empty;

        protected Primitive(Name fullName)
        {
            FullName = fullName;
        }
    }
}
