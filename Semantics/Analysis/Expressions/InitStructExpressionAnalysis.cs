using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Call;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions
{
    public class InitStructExpressionAnalysis : ExpressionAnalysis
    {
        [NotNull] public new InitStructExpressionSyntax Syntax { get; }
        [NotNull] public ExpressionAnalysis ConstructorExpression { get; set; }
        [NotNull] [ItemNotNull] public IReadOnlyList<ArgumentAnalysis> Arguments { get; }

        public InitStructExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] InitStructExpressionSyntax syntax,
            [NotNull] ExpressionAnalysis constructorExpression,
            [NotNull] [ItemNotNull] IEnumerable<ArgumentAnalysis> arguments)
            : base(context, syntax)
        {
            Requires.NotNull(nameof(constructorExpression), constructorExpression);
            Requires.NotNull(nameof(arguments), arguments);
            Syntax = syntax;
            ConstructorExpression = constructorExpression;
            Arguments = arguments.ToReadOnlyList();
        }
    }
}
