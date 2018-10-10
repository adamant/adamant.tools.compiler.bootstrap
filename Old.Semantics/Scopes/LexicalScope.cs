using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes
{
    public class LexicalScope
    {
        public readonly LexicalScope EnclosingScope;
        public readonly ISyntaxSymbol Symbol;

        public LexicalScope(ISyntaxSymbol symbol)
        {
            Requires.NotNull(nameof(symbol), symbol);
            Symbol = symbol;
        }

        public LexicalScope(LexicalScope enclosingScope, ISyntaxSymbol symbol)
        {
            Requires.NotNull(nameof(enclosingScope), enclosingScope);
            Requires.NotNull(nameof(symbol), symbol);
            EnclosingScope = enclosingScope;
            Symbol = symbol;
        }

        public ISyntaxSymbol LookupName(string name)
        {
            foreach (var childSymbol in Symbol.Children)
                if (childSymbol.Name == name)
                    return childSymbol;

            return EnclosingScope?.LookupName(name);
        }
    }
}
