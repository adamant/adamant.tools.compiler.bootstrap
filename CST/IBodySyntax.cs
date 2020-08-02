using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IBodySyntax : IBodyOrBlockSyntax
    {
        new FixedList<IBodyStatementSyntax> Statements { get; }
    }
}
