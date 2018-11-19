using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public abstract class SimpleNameAnalysis : ExpressionAnalysis
    {
        [NotNull] public SimpleNameSyntax Syntax { get; }
        [NotNull] public SimpleName Name => Syntax.Name;

        protected SimpleNameAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] SimpleNameSyntax syntax)
            : base(context, syntax.Span)
        {
            Syntax = syntax;
        }
    }
}
