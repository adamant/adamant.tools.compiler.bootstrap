using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class BlockAnalysis : ExpressionAnalysis, ILocalVariableScopeAnalysis
    {
        [NotNull] public new BlockSyntax Syntax { get; }
        [NotNull] [ItemNotNull] public IReadOnlyList<StatementAnalysis> Statements { get; }

        public BlockAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] BlockSyntax syntax,
            [NotNull] [ItemNotNull] IEnumerable<StatementAnalysis> statements)
            : base(context, syntax)
        {
            Requires.NotNull(nameof(statements), statements);
            Syntax = syntax;
            Statements = statements.ToReadOnlyList();
        }

        IEnumerable<IDeclarationAnalysis> ILocalVariableScopeAnalysis.LocalVariableDeclarations()
        {
            return Statements.OfType<VariableDeclarationStatementAnalysis>();
        }
    }
}