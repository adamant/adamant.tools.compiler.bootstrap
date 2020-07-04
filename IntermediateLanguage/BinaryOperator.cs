using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public enum BinaryOperator
    {
        Plus,
        Minus,
        Asterisk,
        Slash,
        EqualsEquals,
        NotEqual,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
        And,
        Or,
        DotDot,
        LessThanDotDot,
        DotDotLessThan,
        LessThanDotDotLessThan,
    }

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
