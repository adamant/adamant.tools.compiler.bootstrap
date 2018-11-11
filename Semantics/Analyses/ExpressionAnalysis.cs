using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public abstract class ExpressionAnalysis : SyntaxAnalysis
    {
        public readonly TextSpan Span;
        [NotNull] public TypeAnalysis Type { get; } = new TypeAnalysis();

        protected ExpressionAnalysis(
            [NotNull] AnalysisContext context,
            TextSpan span)
            : base(context)
        {
            Span = span;
        }
    }
}
