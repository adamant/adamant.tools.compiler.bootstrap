using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class BooleanLiteralExpressionAnalysis : LiteralExpressionAnalysis
    {
        public bool Value { get; }

        public BooleanLiteralExpressionAnalysis(
            [NotNull] AnalysisContext context,
            TextSpan span,
            bool value)
            : base(context, span)
        {
            Value = value;
        }
    }
}
