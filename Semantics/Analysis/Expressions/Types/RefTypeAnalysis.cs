using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.Types
{
    public class RefTypeAnalysis : ExpressionAnalysis
    {
        [NotNull] public new RefTypeSyntax Syntax { get; }
        [NotNull] public ExpressionAnalysis ReferencedType { get; }

        public RefTypeAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] RefTypeSyntax syntax,
            [NotNull] ExpressionAnalysis referencedType)
            : base(context, syntax)
        {
            Requires.NotNull(nameof(referencedType), referencedType);
            Syntax = syntax;
            ReferencedType = referencedType;
        }
    }
}
