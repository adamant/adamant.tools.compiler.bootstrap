using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class NewObjectExpressionAnalysis : ExpressionAnalysis
    {
        [NotNull] public NewObjectExpressionSyntax Syntax { get; }
        [NotNull] public ExpressionAnalysis ConstructorExpression { get; set; }
        [NotNull, ItemNotNull] public FixedList<ArgumentAnalysis> Arguments { get; }

        public NewObjectExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] NewObjectExpressionSyntax syntax,
            [NotNull] ExpressionAnalysis constructorExpression,
            [NotNull, ItemNotNull] IEnumerable<ArgumentAnalysis> arguments)
            : base(context, syntax.Span)
        {
            Syntax = syntax;
            ConstructorExpression = constructorExpression;
            Arguments = arguments.ToFixedList();
        }
    }
}
