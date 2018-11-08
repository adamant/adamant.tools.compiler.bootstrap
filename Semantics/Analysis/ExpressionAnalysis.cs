using Adamant.Tools.Compiler.Bootstrap.Semantics.TypeChecking;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
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
