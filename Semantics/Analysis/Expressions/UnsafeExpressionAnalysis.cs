using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions
{
    public class UnsafeExpressionAnalysis : ExpressionAnalysis
    {
        [NotNull] public new UnsafeExpressionSyntax Syntax { get; }
        [NotNull] public ExpressionAnalysis Expression { get; }

        public UnsafeExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] UnsafeExpressionSyntax syntax,
            [NotNull] ExpressionAnalysis expression)
            : base(context, syntax)
        {
            Requires.NotNull(nameof(expression), expression);
            Syntax = syntax;
            Expression = expression;
        }
    }
}
