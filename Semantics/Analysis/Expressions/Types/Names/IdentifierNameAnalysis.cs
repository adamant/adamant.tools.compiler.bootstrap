using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.Types.Names
{
    public class IdentifierNameAnalysis : ExpressionAnalysis
    {
        [NotNull] public new IdentifierNameSyntax Syntax { get; }
        [CanBeNull] public string Name => Syntax.Name.Value;

        public IdentifierNameAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] IdentifierNameSyntax syntax)
            : base(context, syntax)
        {
            Syntax = syntax;
        }
    }
}
