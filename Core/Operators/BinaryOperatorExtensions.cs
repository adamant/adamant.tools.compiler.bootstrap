using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Core.Operators
{
    public static class BinaryOperatorExtensions
    {
        public static string ToSymbolString(this BinaryOperator @operator)
        {
            return @operator switch
            {
                BinaryOperator.Plus => "+",
                BinaryOperator.Minus => "-",
                BinaryOperator.Asterisk => "*",
                BinaryOperator.Slash => "/",
                BinaryOperator.EqualsEquals => "==",
                BinaryOperator.NotEqual => "=/=",
                BinaryOperator.LessThan => "<",
                BinaryOperator.LessThanOrEqual => "<=",
                BinaryOperator.GreaterThan => ">",
                BinaryOperator.GreaterThanOrEqual => ">=",
                BinaryOperator.And => "and",
                BinaryOperator.Or => "or",
                BinaryOperator.DotDot => "..",
                BinaryOperator.LessThanDotDot => "<..",
                BinaryOperator.DotDotLessThan => "..<",
                BinaryOperator.LessThanDotDotLessThan => "<..<",
                _ => throw ExhaustiveMatch.Failed(@operator)
            };
        }
    }
}
