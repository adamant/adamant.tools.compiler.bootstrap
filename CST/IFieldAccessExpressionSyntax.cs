namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IFieldAccessExpressionSyntax
    {
        ref IExpressionSyntax ContextExpression { get; }
    }
}
