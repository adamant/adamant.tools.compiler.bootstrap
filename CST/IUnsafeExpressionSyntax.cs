namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IUnsafeExpressionSyntax : IExpressionSyntax
    {
        ref IExpressionSyntax Expression { get; }
    }
}
