using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class ExpressionAnalysisBuilder
    {
        [NotNull]
        public ExpressionAnalysis PrepareForAnalysis(
            [NotNull] FunctionDeclarationAnalysis function,
            [NotNull] ExpressionSyntax expression)
        {
            return new ExpressionAnalysis(function.File, function.Scope, expression);
        }
    }
}
