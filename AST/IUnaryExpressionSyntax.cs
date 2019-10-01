using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IUnaryExpressionSyntax : IExpressionSyntax
    {
        UnaryOperatorFixity Fixity { get; }
        UnaryOperator Operator { get; }
        IExpressionSyntax Operand { get; }
        ref IExpressionSyntax OperandRef { get; }
    }
}
