using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Binders
{
    public class NameScope
    {
        public readonly NameScope Parent;
        public readonly ISyntaxSymbol Symbol;

        public NameScope(ISyntaxSymbol symbol)
        {
            Requires.NotNull(nameof(symbol), symbol);
            Symbol = symbol;
        }

        public NameScope(NameScope parent, ISyntaxSymbol symbol)
        {
            Requires.NotNull(nameof(symbol), symbol);
            Parent = parent;
            Symbol = symbol;
        }

        public ISyntaxSymbol LookupName(string name)
        {
            foreach (var childSymbol in Symbol.Children)
            {
                if (childSymbol.Name == name)
                    return childSymbol;
            }

            return Parent?.LookupName(name);
        }
    }
}
