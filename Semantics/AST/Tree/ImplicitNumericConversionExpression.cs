using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class ImplicitNumericConversionExpression : ImplicitConversionExpression, IImplicitNumericConversionExpression
    {
        public NumericType ConvertToType { get; }

        public ImplicitNumericConversionExpression(
            TextSpan span,
            DataType dataType,
            ExpressionSemantics semantics,
            IExpression expression,
            NumericType convertToType)
            : base(span, dataType, semantics, expression)
        {
            ConvertToType = convertToType;
        }

        public override string ToString()
        {
            return $"{{Expression.ToGroupedString(OperatorPrecedence.Min)}} ⟦as {ConvertToType}⟧";
        }
    }
}
