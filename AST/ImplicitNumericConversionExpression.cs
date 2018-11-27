using Adamant.Tools.Compiler.Bootstrap.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class ImplicitNumericConversionExpression : ImplicitConversionExpression
    {
        [NotNull] public ExpressionSyntax Expression { get; }
        [NotNull] public NumericType ConvertToType { get; }

        public ImplicitNumericConversionExpression(
            [NotNull] ExpressionSyntax expression,
            [NotNull] NumericType convertToType)
            : base(expression.Span, convertToType)
        {
            Expression = expression;
            ConvertToType = convertToType;
        }

        public override string ToString()
        {
            return $"({Expression}) (as) {ConvertToType}";
        }
    }
}
