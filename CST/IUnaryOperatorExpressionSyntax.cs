using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IUnaryOperatorExpressionSyntax : IExpressionSyntax
    {
        UnaryOperatorFixity Fixity { get; }
        UnaryOperator Operator { get; }
        ref IExpressionSyntax Operand { get; }
    }
}
