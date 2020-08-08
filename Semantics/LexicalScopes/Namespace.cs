using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.LexicalScopes
{
    internal class Namespace
    {
        public NamespaceName Name { get; }
        public FixedDictionary<TypeName, FixedList<Promise<Symbol?>>> Symbols { get; }
        public FixedDictionary<TypeName, FixedList<Promise<Symbol?>>> NestedSymbols { get; }

        public Namespace(
            NamespaceName name,
            FixedDictionary<TypeName, FixedList<Promise<Symbol?>>> symbols,
            FixedDictionary<TypeName, FixedList<Promise<Symbol?>>> nestedSymbols)
        {
            Name = name;
            Symbols = symbols;
            NestedSymbols = nestedSymbols;
        }
    }
}
