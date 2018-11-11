using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class MutableTypeAnalysis : ExpressionAnalysis
    {
        [NotNull] public new MutableTypeSyntax Syntax { get; }
        [NotNull] public ExpressionAnalysis ReferencedType { get; }

        public MutableTypeAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] MutableTypeSyntax syntax,
            [NotNull] ExpressionAnalysis referencedType)
            : base(context, syntax.Span)
        {
            Requires.NotNull(nameof(referencedType), referencedType);
            Syntax = syntax;
            ReferencedType = referencedType;
        }
    }
}
