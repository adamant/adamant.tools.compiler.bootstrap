using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions
{
    public abstract class ExpressionAnalysis : AnalysisNode
    {
        [NotNull] public new ExpressionSyntax Syntax { get; }
        [CanBeNull] public DataType Type { get; internal set; }

        protected ExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] ExpressionSyntax syntax)
            : base(context, syntax)
        {
            Syntax = syntax;
        }
    }
}