using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IUnaryOperatorExpressionSyntax : IExpressionSyntax
    {
        UnaryOperatorFixity Fixity { get; }
        UnaryOperator Operator { get; }
        ref IExpressionSyntax Operand { get; }
    }
}
