using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class RefTypeAnalysis : ExpressionAnalysis
    {
        [NotNull] public new RefTypeSyntax Syntax { get; }
        public bool VariableBinding => Syntax.VarKeyword != null;
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