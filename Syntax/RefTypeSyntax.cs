using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class RefTypeSyntax : TypeSyntax
    {
        [NotNull] public IRefKeywordToken RefKeyword { get; }
        [CanBeNull] public IVarKeywordToken VarKeyword { get; }
        [NotNull] public ExpressionSyntax ReferencedType { get; }

        public RefTypeSyntax(
            [NotNull] IRefKeywordToken refKeyword,
            [CanBeNull] IVarKeywordToken varKeyword,
            [NotNull] ExpressionSyntax referencedType)
            : base(TextSpan.Covering(refKeyword.Span, referencedType.Span))
        {
            RefKeyword = refKeyword;
            VarKeyword = varKeyword;
            ReferencedType = referencedType;
        }

        public override string ToString()
        {
            return $"ref var {ReferencedType}";
        }
    }
}
