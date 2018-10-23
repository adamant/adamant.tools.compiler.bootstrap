using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions
{
    public class BlockExpressionAnalysis : ExpressionAnalysis
    {
        [NotNull] public new BlockExpressionSyntax Syntax { get; }
        [NotNull] [ItemNotNull] public IReadOnlyList<StatementAnalysis> Statements { get; }

        public BlockExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] BlockExpressionSyntax syntax,
            [NotNull] [ItemNotNull] IEnumerable<StatementAnalysis> statements)
            : base(context, syntax)
        {
            Requires.NotNull(nameof(statements), statements);
            Syntax = syntax;
            Statements = statements.ToReadOnlyList();
        }
    }
}
