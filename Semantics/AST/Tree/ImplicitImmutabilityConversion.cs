using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class ImplicitImmutabilityConversion : ImplicitConversionExpression, IImplicitImmutabilityConversionExpression
    {
        public ObjectType ConvertToType { get; }

        public ImplicitImmutabilityConversion(
            TextSpan span,
            DataType dataType,
            ExpressionSemantics semantics,
            IExpression expression,
            ObjectType convertToType)
            : base(span, dataType, semantics, expression)
        {
            ConvertToType = convertToType;
        }

        public override string ToString()
        {
            return $"{Expression.ToGroupedString(OperatorPrecedence.Min)} ⟦as ⟦immutable⟧⟧";
        }
    }
}
