namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IIfExpressionSyntax
    {
        ref IExpressionSyntax Condition { get; }
    }
}
