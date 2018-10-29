using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types
{
    public class MutableTypeSyntax : TypeSyntax
    {
        [NotNull] public MutableKeywordToken RefKeyword { get; }
        [NotNull] public ExpressionSyntax ReferencedType { get; }

        public MutableTypeSyntax([NotNull] MutableKeywordToken refKeyword, [NotNull] ExpressionSyntax referencedType)
            : base(TextSpan.Covering(refKeyword.Span, referencedType.Span))
        {
            RefKeyword = refKeyword;
            ReferencedType = referencedType;
        }
    }
}
