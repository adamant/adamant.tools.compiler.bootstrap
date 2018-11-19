using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols
{
    public class CompositeSymbol : ISymbol
    {
        [NotNull] public Name Name { get; }
        [NotNull] [ItemNotNull] public FixedList<ISymbol> Symbols { get; }

        public CompositeSymbol([NotNull] ISymbol symbol1, [NotNull] ISymbol symbol2)
        {
            Requires.NotNull(nameof(symbol1), symbol1);
            Requires.NotNull(nameof(symbol2), symbol2);
            Requires.That(nameof(Name), symbol1.Name.Equals(symbol2.Name));
            Name = symbol1.Name;
            Symbols = new[] { symbol1, symbol2 }.ToFixedList();
        }

        private CompositeSymbol(
            [NotNull] Name name,
            [NotNull][ItemNotNull]  IEnumerable<ISymbol> symbols)
        {
            Requires.NotNull(nameof(name), name);
            Requires.NotNull(nameof(symbols), symbols);
            Name = name;
            Symbols = symbols.ToFixedList();
        }

        public IEnumerable<DataType> Types => Symbols.SelectMany(s => s.Types);

        [NotNull]
        public ISymbol ComposeWith([NotNull] ISymbol symbol)
        {
            Requires.That(nameof(symbol), Name.Equals(symbol.Name));
            return new CompositeSymbol(Name, Symbols.Append(symbol).NotNull());
        }

        [CanBeNull]
        public ISymbol Lookup([NotNull] SimpleName name)
        {
            throw new System.NotImplementedException();
        }
    }
}
