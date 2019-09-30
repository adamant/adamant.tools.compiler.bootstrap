using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(IVariableDeclarationStatementSyntax),
        typeof(IExpressionStatementSyntax),
        typeof(IResultStatementSyntax))]
    public interface IStatementSyntax : ISyntax
    {

    }
}
