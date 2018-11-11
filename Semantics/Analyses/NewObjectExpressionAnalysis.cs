using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class NewObjectExpressionAnalysis : ExpressionAnalysis
    {
        [NotNull] public new NewObjectExpressionSyntax Syntax { get; }
        [NotNull] public ExpressionAnalysis ConstructorExpression { get; set; }
        [NotNull, ItemNotNull] public IReadOnlyList<ArgumentAnalysis> Arguments { get; }

        public NewObjectExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] NewObjectExpressionSyntax syntax,
            [NotNull] ExpressionAnalysis constructorExpression,
            [NotNull, ItemNotNull] IEnumerable<ArgumentAnalysis> arguments)
            : base(context, syntax.Span)
        {
            Requires.NotNull(nameof(constructorExpression), constructorExpression);
            Requires.NotNull(nameof(arguments), arguments);
            Syntax = syntax;
            ConstructorExpression = constructorExpression;
            Arguments = arguments.ToReadOnlyList();
        }
    }
}
