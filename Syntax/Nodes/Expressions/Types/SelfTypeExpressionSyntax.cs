using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types
{
    public class SelfTypeExpressionSyntax : TypeSyntax
    {
        [NotNull] public SelfTypeKeywordToken SelfTypeKeyword { get; }

        public SelfTypeExpressionSyntax([NotNull] SelfTypeKeywordToken selfTypeKeyword)
            : base(selfTypeKeyword.Span)
        {
            Requires.NotNull(nameof(selfTypeKeyword), selfTypeKeyword);
            SelfTypeKeyword = selfTypeKeyword;
        }
    }
}
