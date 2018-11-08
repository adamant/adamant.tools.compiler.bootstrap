using System.Numerics;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class IntegerLiteralExpressionAnalysis : LiteralExpressionAnalysis
    {
        [NotNull] public new IntegerLiteralExpressionSyntax Syntax { get; }
        public BigInteger Value => Syntax.Literal.Value;

        public IntegerLiteralExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] IntegerLiteralExpressionSyntax syntax)
            : base(context, syntax)
        {
            Syntax = syntax;
        }
    }
}