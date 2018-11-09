using Adamant.Tools.Compiler.Bootstrap.Syntax;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public abstract class DeclarationAnalysis : SyntaxAnalysis
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
