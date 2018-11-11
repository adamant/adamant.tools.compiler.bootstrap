using Adamant.Tools.Compiler.Bootstrap.Syntax;
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
