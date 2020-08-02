namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IStringLiteralExpressionSyntax : ILiteralExpressionSyntax
    {
        string Value { get; }
    }
}
