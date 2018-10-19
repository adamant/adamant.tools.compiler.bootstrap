using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.Types
{
    public class LifetimeTypeAnalysis : ExpressionAnalysis
    {
        [NotNull] public new LifetimeTypeSyntax Syntax { get; }
        [NotNull] public ExpressionAnalysis TypeName { get; }

        public LifetimeTypeAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] LifetimeTypeSyntax syntax,
            [NotNull] ExpressionAnalysis typeName)
            : base(context, syntax)
        {
            Requires.NotNull(nameof(typeName), typeName);
            Syntax = syntax;
            TypeName = typeName;
        }
    }
}
