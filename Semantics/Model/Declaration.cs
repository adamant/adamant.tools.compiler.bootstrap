using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Model
{
    public abstract class Declaration : ISymbol
    {
        [NotNull] public Name Name { get; }
        [NotNull] public DataType Type { get; }

        protected Declaration(
            [NotNull] Name name,
            [NotNull] DataType type)
        {
            Name = name;
            Type = type;
        }

        IEnumerable<DataType> ISymbol.Types => Type.Yield();
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
