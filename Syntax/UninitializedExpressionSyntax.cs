using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class UninitializedExpressionSyntax : LiteralExpressionSyntax
    {
        [NotNull] public IUninitializedKeywordToken UninitializedKeyword { get; }

        public UninitializedExpressionSyntax([NotNull] IUninitializedKeywordToken uninitializedKeyword)
            : base(uninitializedKeyword.Span)
        {
            Requires.NotNull(nameof(uninitializedKeyword), uninitializedKeyword);
            UninitializedKeyword = uninitializedKeyword;
        }
    }
}
