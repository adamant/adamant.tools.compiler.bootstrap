using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IBodySyntax : IBodyOrBlockSyntax
    {
        new FixedList<IBodyStatementSyntax> Statements { get; }
    }
}
