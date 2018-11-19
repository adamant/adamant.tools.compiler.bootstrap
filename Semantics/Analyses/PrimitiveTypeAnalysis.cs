using Adamant.Tools.Compiler.Bootstrap.AST;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class PrimitiveTypeAnalysis : ExpressionAnalysis
    {
        [NotNull] public new PrimitiveTypeSyntax Syntax { get; }

        public PrimitiveTypeAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] PrimitiveTypeSyntax syntax)
            : base(context, syntax.Span)
        {
            Syntax = syntax;
        }
    }
}
