using System.Numerics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class IntegerLiteralExpressionAnalysis : LiteralExpressionAnalysis
    {
        public BigInteger Value { get; }

        public IntegerLiteralExpressionAnalysis(
            [NotNull] AnalysisContext context,
            TextSpan span,
            BigInteger value)
            : base(context, span)
        {
            Value = value;
        }
    }
}
