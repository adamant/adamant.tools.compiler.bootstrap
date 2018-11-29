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

        protected Declaration(
            [NotNull] Name fullName,
            [NotNull] DataType type)
        {
            FullName = fullName;
            Type = type;
        }

        ISymbol ISymbol.ComposeWith([NotNull] ISymbol symbol)
        {
            if (symbol is CompositeSymbol composite)
                return composite.ComposeWith(this);
            return new CompositeSymbol(this, symbol);
        }

        [CanBeNull]
        public abstract ISymbol Lookup([NotNull] SimpleName name);
    }
}
