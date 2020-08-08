using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.LexicalScopes
{
    internal readonly struct NameAndSymbol
    {

        public TypeName Name { get; }
        public Promise<Symbol?> Symbol { get; }

        public NameAndSymbol(TypeName name, Promise<Symbol?> symbol)
        {
            Name = name;
            Symbol = symbol;
        }

        public NameAndSymbol(TypeName name, Symbol? symbol)
        {
            Name = name;
            Symbol = Promise.ForValue(symbol);
        }
    }
}
