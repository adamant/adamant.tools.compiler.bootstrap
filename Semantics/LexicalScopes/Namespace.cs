using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.LexicalScopes
{
    internal class Namespace
    {
        public NamespaceName Name { get; }
        public FixedDictionary<TypeName, FixedSet<IPromise<Symbol>>> Symbols { get; }
        public FixedDictionary<TypeName, FixedSet<IPromise<Symbol>>> NestedSymbols { get; }

        public Namespace(
            NamespaceName name,
            FixedDictionary<TypeName, FixedSet<IPromise<Symbol>>> symbols,
            FixedDictionary<TypeName, FixedSet<IPromise<Symbol>>> nestedSymbols)
        {
            Name = name;
            Symbols = symbols;
            NestedSymbols = nestedSymbols;
        }
    }
}
