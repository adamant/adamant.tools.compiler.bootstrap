using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class MutableTypeSyntax : TypeSyntax
    {
        [NotNull] public IMutableKeywordToken RefKeyword { get; }
        [NotNull] public ExpressionSyntax ReferencedType { get; }

        public MutableTypeSyntax([NotNull] IMutableKeywordToken refKeyword, [NotNull] ExpressionSyntax referencedType)
            : base(TextSpan.Covering(refKeyword.Span, referencedType.Span))
        {
            RefKeyword = refKeyword;
            ReferencedType = referencedType;
        }

        public override string ToString()
        {
            return $"ref {ReferencedType}";
        }
    }
}
