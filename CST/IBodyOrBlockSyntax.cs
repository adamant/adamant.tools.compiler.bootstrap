using Adamant.Tools.Compiler.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    [Closed(
        typeof(IBodySyntax),
        typeof(IBlockExpressionSyntax))]
    public partial interface IBodyOrBlockSyntax : ISyntax
    {
        FixedList<IStatementSyntax> Statements { get; }
    }
}
