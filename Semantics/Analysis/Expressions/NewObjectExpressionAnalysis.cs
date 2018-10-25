using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions
{
    public class NewObjectExpressionAnalysis : ExpressionAnalysis
    {
        [NotNull] public new NewObjectExpressionSyntax Syntax { get; }
        [NotNull] [ItemNotNull] public IReadOnlyList<ExpressionAnalysis> Arguments { get; }

        public NewObjectExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] NewObjectExpressionSyntax syntax,
            [NotNull] [ItemNotNull] IEnumerable<ExpressionAnalysis> arguments)
            : base(context, syntax)
        {
            Syntax = syntax;
            Arguments = arguments.ToReadOnlyList();
        }
    }
}
