using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class UnsafeExpressionAnalysis : ExpressionAnalysis
    {
        [NotNull] public new UnsafeExpressionSyntax Syntax { get; }
        [NotNull] public ExpressionAnalysis Expression { get; }

        public UnsafeExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] UnsafeExpressionSyntax syntax,
            [NotNull] ExpressionAnalysis expression)
            : base(context, syntax.Span)
        {
            Requires.NotNull(nameof(expression), expression);
            Syntax = syntax;
            Expression = expression;
        }
    }
}
