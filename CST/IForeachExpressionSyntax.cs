namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IForeachExpressionSyntax
    {
        bool VariableIsLiveAfterAssignment { get; set; }
    }
}
