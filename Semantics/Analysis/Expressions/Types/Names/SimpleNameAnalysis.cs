using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.Types.Names
{
    public abstract class SimpleNameAnalysis : ExpressionAnalysis
    {
        [NotNull] public new SimpleNameSyntax Syntax { get; }
        [CanBeNull] public string Name => Syntax.Name.Value;

        protected SimpleNameAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] SimpleNameSyntax syntax)
            : base(context, syntax)
        {
            Syntax = syntax;
        }
    }
}