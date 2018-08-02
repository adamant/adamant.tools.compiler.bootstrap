using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes
{
    public class LexicalScope
    {
        public readonly LexicalScope EnclosingScope;
        public readonly SyntaxSymbol Symbol;

        public LexicalScope(SyntaxSymbol symbol)
        {
            Requires.NotNull(nameof(symbol), symbol);
            Symbol = symbol;
        }

        public LexicalScope(LexicalScope enclosingScope, SyntaxSymbol symbol)
        {
            Requires.NotNull(nameof(symbol), symbol);
            EnclosingScope = enclosingScope;
            Symbol = symbol;
        }

        public SyntaxSymbol LookupName(string name)
        {
            foreach (var childSymbol in Symbol.Children)
                if (childSymbol.Name == name)
                    return childSymbol;

            return EnclosingScope?.LookupName(name);
        }
    }
}
