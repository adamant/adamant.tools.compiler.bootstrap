using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IBlockSyntax : IExpressionSyntax, IBlockOrResultSyntax
    {
        FixedList<IStatementSyntax> Statements { get; }
    }
}
