using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class MatchExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public ExpressionSyntax Value { get; }
        [NotNull] public FixedList<MatchArmSyntax> Arms { get; }

        public MatchExpressionSyntax(
            TextSpan span,
            [NotNull] ExpressionSyntax value,
            [NotNull] FixedList<MatchArmSyntax> arms)
            : base(span)
        {
            Value = value;
            Arms = arms;
        }

        public override string ToString()
        {
            return $"match {Value} {{{Arms}}}";
        }
    }
}
