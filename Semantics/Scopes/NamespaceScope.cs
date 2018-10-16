using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes
{
    public class NamespaceScope : LexicalScope
    {
        [NotNull] public LexicalScope ContainingScope { get; }

        public NamespaceScope([NotNull] LexicalScope containingScope)
        {
            Requires.NotNull(nameof(containingScope), containingScope);
            ContainingScope = containingScope;
        }
    }
}
