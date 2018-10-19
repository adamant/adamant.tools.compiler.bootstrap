using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Statements
{
    public abstract class StatementAnalysis
    {
        [NotNull] public AnalysisContext Context { get; }

        protected StatementAnalysis([NotNull] AnalysisContext context)
        {
            Requires.NotNull(nameof(context), context);
            Context = context;
        }
    }
}
