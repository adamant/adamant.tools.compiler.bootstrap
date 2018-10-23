using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes
{
    public abstract class DeclarationScope : LexicalScope
    {
        [NotNull] public LexicalScope ContainingScope { get; }

        protected DeclarationScope(
            [NotNull] LexicalScope containingScope,
            [NotNull] SyntaxNode syntax)
            : base(syntax)
        {
            Requires.NotNull(nameof(containingScope), containingScope);
            ContainingScope = containingScope;
            containingScope.Add(this);
        }
    }
}
