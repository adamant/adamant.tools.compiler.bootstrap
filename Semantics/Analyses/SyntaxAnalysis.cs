using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public abstract class SyntaxAnalysis
    {
        [NotNull] public AnalysisContext Context { get; }
        [NotNull] public SyntaxNode Syntax { get; }

        protected SyntaxAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] SyntaxNode syntax)
        {
            Requires.NotNull(nameof(context), context);
            Requires.NotNull(nameof(syntax), syntax);
            Context = context;
            Syntax = syntax;
        }
    }
}
