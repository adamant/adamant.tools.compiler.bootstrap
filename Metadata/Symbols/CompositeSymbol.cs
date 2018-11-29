using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols
{
    // TODO split this out so symbols are always singular somehow
    public class CompositeSymbol : ISymbol
    {
        [NotNull] public Name FullName { get; }
        [NotNull] [ItemNotNull] public FixedList<ISymbol> Symbols { get; }
        DataType ISymbol.Type => throw new NotImplementedException();

        public CompositeSymbol([NotNull] ISymbol symbol1, [NotNull] ISymbol symbol2)
        {
            Requires.NotNull(nameof(symbol1), symbol1);
            Requires.NotNull(nameof(symbol2), symbol2);
            Requires.That(nameof(FullName), symbol1.FullName.Equals(symbol2.FullName));
            FullName = symbol1.FullName;
            Symbols = new[] { symbol1, symbol2 }.ToFixedList();
        }

        private CompositeSymbol(
            [NotNull] Name name,
            [NotNull][ItemNotNull]  IEnumerable<ISymbol> symbols)
        {
            Requires.NotNull(nameof(name), name);
            Requires.NotNull(nameof(symbols), symbols);
            FullName = name;
            Symbols = symbols.ToFixedList();
        }

        [NotNull]
        public ISymbol ComposeWith([NotNull] ISymbol symbol)
        {
            Requires.That(nameof(symbol), FullName.Equals(symbol.FullName));
            return new CompositeSymbol(FullName, Symbols.Append(symbol).NotNull());
        }

        [CanBeNull]
        public ISymbol Lookup([NotNull] SimpleName name)
        {
            throw new System.NotImplementedException();
        }
    }
}
