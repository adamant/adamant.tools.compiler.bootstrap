using Adamant.Tools.Compiler.Bootstrap.Core;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(VariableDeclarationStatementSyntax),
        typeof(ExpressionStatementSyntax),
        typeof(ResultStatementSyntax))]
    public abstract class StatementSyntax : Syntax
    {
        private protected StatementSyntax(TextSpan span)
            : base(span)
        {
        }
    }
}
