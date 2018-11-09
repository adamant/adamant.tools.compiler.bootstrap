using Adamant.Tools.Compiler.Bootstrap.Syntax;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class StringLiteralExpressionAnalysis : LiteralExpressionAnalysis
    {
        [NotNull] public new StringLiteralExpressionSyntax Syntax { get; }
        public string Value => Syntax.Literal.Value;

        public StringLiteralExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] StringLiteralExpressionSyntax syntax)
            : base(context, syntax)
        {
            Syntax = syntax;
        }
    }
}
