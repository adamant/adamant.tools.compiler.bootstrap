using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class BlockAnalysis : ExpressionAnalysis, ILocalVariableScopeAnalysis
    {
        [NotNull] public new BlockSyntax Syntax { get; }
        [NotNull] [ItemNotNull] public IReadOnlyList<StatementAnalysis> Statements { get; }

        public BlockAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] BlockSyntax syntax,
            [NotNull] [ItemNotNull] IEnumerable<StatementAnalysis> statements)
            : base(context, syntax.Span)
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
