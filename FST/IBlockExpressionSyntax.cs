namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    /// <summary>
    /// A block expression. Not to be used to represent function
    /// or type bodies.
    /// </summary>
    public interface IBlockExpressionSyntax : IExpressionSyntax, IBlockOrResultSyntax, IBodyOrBlockSyntax
    {
    }
}
