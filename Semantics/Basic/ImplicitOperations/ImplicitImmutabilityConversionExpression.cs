using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic.ImplicitOperations
{
    // TODO No error is reported if IImplicitImmutabilityConversionExpression is missing
    internal class ImplicitImmutabilityConversionExpression : ImplicitConversionExpression, IImplicitImmutabilityConversionExpression
    {
        public ImplicitImmutabilityConversionExpression(
            IExpressionSyntax expression,
            ObjectType convertToType)
            : base(expression.Span, convertToType, expression)
        {
        }

        public override string ToString()
        {
            return $"{Expression.ToGroupedString(OperatorPrecedence.Min)} ⟦as ⟦immutable⟧⟧";
        }
    }
}
