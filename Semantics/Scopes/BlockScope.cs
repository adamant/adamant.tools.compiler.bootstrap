using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Blocks;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes
{
    public class BlockScope : NestedScope
    {
        [NotNull] public new BlockSyntax Syntax { get; }

        public BlockScope(
            [NotNull] LexicalScope containingScope,
            [NotNull] BlockSyntax syntax)
            : base(containingScope, syntax)
        {
            Syntax = syntax;
        }
    }
}
