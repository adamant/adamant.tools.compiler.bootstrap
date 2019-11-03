using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(IBodyStatementSyntax),
        typeof(IResultStatementSyntax))]
    public interface IStatementSyntax : ISyntax
    {
    }
}
