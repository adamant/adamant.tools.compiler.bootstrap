using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Model
{
    public abstract class Declaration : ISymbol
    {
        [NotNull] public Name FullName { get; }
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [NotNull] public SimpleName Name => FullName.UnqualifiedName;
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [NotNull] public SimpleName LookupByName => FullName.UnqualifiedName;

        [NotNull] public DataType Type { get; }
        [NotNull] public SymbolSet ChildSymbols { get; }

        protected Declaration(
            [NotNull] Name fullName,
            [NotNull] DataType type,
            [NotNull] SymbolSet childSymbols)
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
