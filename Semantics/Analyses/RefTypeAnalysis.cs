using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class RefTypeAnalysis : ExpressionAnalysis
    {
        [NotNull] public RefTypeSyntax Syntax { get; }
        [NotNull] public ExpressionAnalysis ReferencedType { get; }

        public RefTypeAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] RefTypeSyntax syntax,
            [NotNull] ExpressionAnalysis referencedType)
            : base(context, syntax.Span)
        {
            Requires.NotNull(nameof(referencedType), referencedType);
            Syntax = syntax;
            ReferencedType = referencedType;
        }
    }
}
