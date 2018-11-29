using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Model
{
    public abstract class Declaration : ISymbol
    {
        [NotNull] public Name FullName { get; }
        [NotNull] public DataType Type { get; }
        [NotNull] public FixedDictionary<SimpleName, ISymbol> ChildSymbols { get; }

        protected Declaration(
            [NotNull] Name fullName,
            [NotNull] DataType type,
            [NotNull] FixedDictionary<SimpleName, ISymbol> childSymbols)
        {
            FullName = fullName;
            Type = type;
            ChildSymbols = childSymbols;
        }
    }
}
