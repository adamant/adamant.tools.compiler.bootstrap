using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.Types
{
    public class PrimitiveTypeAnalysis : ExpressionAnalysis
    {
        [NotNull] public new PrimitiveTypeSyntax Syntax { get; }

        public PrimitiveTypeAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] PrimitiveTypeSyntax syntax)
            : base(context, syntax)
        {
            Syntax = syntax;
        }
    }
}
