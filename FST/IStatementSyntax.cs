using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    [Closed(
        typeof(IBodyStatementSyntax),
        typeof(IResultStatementSyntax))]
    public interface IStatementSyntax : ISyntax
    {
    }
}
