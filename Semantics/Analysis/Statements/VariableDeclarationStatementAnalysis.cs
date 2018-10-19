using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Statements
{
    public class VariableDeclarationStatementAnalysis : StatementAnalysis
    {
        [NotNull] public VariableDeclarationStatementSyntax Syntax { get; }

        public VariableDeclarationStatementAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] VariableDeclarationStatementSyntax syntax)
            : base(context)
        {
            Requires.NotNull(nameof(syntax), syntax);
            Syntax = syntax;
        }
    }
}
