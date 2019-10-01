using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    /// <summary>
    /// A block expression. Not to be used to represent function
    /// or type bodies.
    /// </summary>
    public interface IBlockExpressionSyntax : IExpressionSyntax, IBlockOrResultSyntax
    {
        FixedList<IStatementSyntax> Statements { get; }
    }
}
