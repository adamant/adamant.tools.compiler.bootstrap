using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public abstract class Declaration : ISymbol
    {
        public Name FullName { get; }
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public SimpleName Name => FullName.UnqualifiedName;

        public DataType Type { get; }
        public SymbolSet ChildSymbols { get; }

        protected Declaration(
            Name fullName,
            DataType type,
            SymbolSet childSymbols)
        {
            FullName = fullName;
            Type = type;
            ChildSymbols = childSymbols;
        }

        public override string ToString()
        {
            return FullName.ToString();
        }
    }
}
