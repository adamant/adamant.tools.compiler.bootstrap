using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations
{
    public abstract class DeclarationAnalysis : AnalysisNode
    {
        [NotNull] public new DeclarationSyntax Syntax { get; }

        protected DeclarationAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] DeclarationSyntax syntax)
            : base(context, syntax)
        {
            Syntax = syntax;
        }
    }
}
