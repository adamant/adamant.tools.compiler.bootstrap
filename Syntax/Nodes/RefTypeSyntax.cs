using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class RefTypeSyntax : TypeSyntax
    {
        [NotNull] public RefKeywordToken RefKeyword { get; }
        [CanBeNull] public VarKeywordToken VarKeyword { get; }
        [NotNull] public ExpressionSyntax ReferencedType { get; }

        public RefTypeSyntax(
            [NotNull] RefKeywordToken refKeyword,
            [CanBeNull] VarKeywordToken varKeyword,
            [NotNull] ExpressionSyntax referencedType)
            : base(TextSpan.Covering(refKeyword.Span, referencedType.Span))
        {
            RefKeyword = refKeyword;
            VarKeyword = varKeyword;
            ReferencedType = referencedType;
        }
    }
}
