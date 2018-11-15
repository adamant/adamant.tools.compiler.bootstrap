using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class UnsafeExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public IUnsafeKeywordToken UnsafeKeyword { get; }
        [NotNull] public ExpressionSyntax Expression { get; }

        public UnsafeExpressionSyntax(
            [NotNull] IUnsafeKeywordToken unsafeKeyword,
            [NotNull] ExpressionSyntax expression)
            : base(TextSpan.Covering(unsafeKeyword.Span, expression.Span))
        {
            UnsafeKeyword = unsafeKeyword;
            Expression = expression;
        }

        public override string ToString()
        {
            return $"unsafe ({Expression})";
        }
    }
}
