using Adamant.Tools.Compiler.Bootstrap.Syntax;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public abstract class ExpressionAnalysis : SyntaxAnalysis
    {
        [NotNull] public new ExpressionSyntax Syntax { get; }
        // This MUST be a field so that we can update the
        [NotNull] public TypeAnalysis Type { get; } = new TypeAnalysis();

        protected ExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] ExpressionSyntax syntax)
            : base(context, syntax)
        {
            Syntax = syntax;
        }
    }
}
