using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    [Closed(
        typeof(IBodyStatementSyntax),
        typeof(IResultStatementSyntax))]
    public interface IStatementSyntax : ISyntax
    {
    }
}
