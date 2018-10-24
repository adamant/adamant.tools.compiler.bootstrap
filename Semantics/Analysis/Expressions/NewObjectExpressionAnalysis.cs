using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions
{
    public class NewObjectExpressionAnalysis : ExpressionAnalysis
    {
        [NotNull] public new NewObjectExpressionSyntax Syntax { get; }

        public NewObjectExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] NewObjectExpressionSyntax syntax)
            : base(context, syntax)
        {
            Syntax = syntax;
        }
    }
}
