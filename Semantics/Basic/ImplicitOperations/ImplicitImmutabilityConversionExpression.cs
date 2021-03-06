using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic.ImplicitOperations
{
    // TODO No error is reported if IImplicitImmutabilityConversionExpression is missing
    internal class ImplicitImmutabilityConversionExpression : ImplicitConversionExpression, IImplicitImmutabilityConversionExpressionSyntax
    {
        public ObjectType ConvertToType { get; }

        public ImplicitImmutabilityConversionExpression(
            IExpressionSyntax expression,
            ObjectType convertToType)
            : base(expression.Span, convertToType, expression, expression.Semantics.Assigned())
        {
            ConvertToType = convertToType;
        }

        public override string ToString()
        {
            return $"{Expression.ToGroupedString(OperatorPrecedence.Min)} ⟦as ⟦immutable⟧⟧";
        }
    }
}
