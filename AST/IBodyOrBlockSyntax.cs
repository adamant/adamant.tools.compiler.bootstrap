using Adamant.Tools.Compiler.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(IBodySyntax),
        typeof(IBlockExpressionSyntax))]
    public interface IBodyOrBlockSyntax : ISyntax
    {
        FixedList<IStatementSyntax> Statements { get; }
    }
}
