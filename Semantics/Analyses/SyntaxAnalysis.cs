using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public abstract class SyntaxAnalysis
    {
        [NotNull] public AnalysisContext Context { get; }

        protected SyntaxAnalysis(
            [NotNull] AnalysisContext context)
        {
            Context = context;
        }
    }
}
