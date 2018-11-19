using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class LifetimeTypeAnalysis : ExpressionAnalysis
    {
        [NotNull] public new LifetimeTypeSyntax Syntax { get; }
        [NotNull] public ExpressionAnalysis TypeName { get; }

        public LifetimeTypeAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] LifetimeTypeSyntax syntax,
            [NotNull] ExpressionAnalysis typeName)
            : base(context, syntax.Span)
        {
            Requires.NotNull(nameof(typeName), typeName);
            Syntax = syntax;
            TypeName = typeName;
        }
    }
}
