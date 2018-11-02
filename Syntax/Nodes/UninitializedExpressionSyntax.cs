using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class UninitializedExpressionSyntax : LiteralExpressionSyntax
    {
        [NotNull] public UninitializedKeywordToken UninitializedKeyword { get; }

        public UninitializedExpressionSyntax([NotNull] UninitializedKeywordToken uninitializedKeyword)
            : base(uninitializedKeyword.Span)
        {
            Requires.NotNull(nameof(uninitializedKeyword), uninitializedKeyword);
            UninitializedKeyword = uninitializedKeyword;
        }
    }
}
