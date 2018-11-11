using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class StringLiteralExpressionAnalysis : LiteralExpressionAnalysis
    {
        public string Value { get; }

        public StringLiteralExpressionAnalysis(
            [NotNull] AnalysisContext context,
            TextSpan span,
            [NotNull] string value)
            : base(context, span)
        {
            Value = value;
        }
    }
}
