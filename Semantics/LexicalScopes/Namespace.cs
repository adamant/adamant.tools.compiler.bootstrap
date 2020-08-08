using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.LexicalScopes
{
    internal class Namespace
    {
        public NamespaceName Name { get; }
        public FixedDictionary<TypeName, FixedSet<Promise<Symbol?>>> Symbols { get; }
        public FixedDictionary<TypeName, FixedSet<Promise<Symbol?>>> NestedSymbols { get; }

        public Namespace(
            NamespaceName name,
            FixedDictionary<TypeName, FixedSet<Promise<Symbol?>>> symbols,
            FixedDictionary<TypeName, FixedSet<Promise<Symbol?>>> nestedSymbols)
        {
            Name = name;
            Symbols = symbols;
            NestedSymbols = nestedSymbols;
        }
    }
}
