namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    /// <summary>
    /// A block expression. Not to be used to represent function
    /// or type bodies.
    /// </summary>
    public interface IBlockExpressionSyntax : IExpressionSyntax, IBlockOrResultSyntax, IBodyOrBlockSyntax
    {
    }
}
