using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions
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
        LessThanDotDotLessThan
    }

    public static class BinaryOperatorExtensions
    {
        public static string ToSymbolString(this BinaryOperator @operator)
        {
            switch (@operator)
            {
                default:
                    throw ExhaustiveMatch.Failed(@operator);
                case BinaryOperator.Plus:
                    return "+";
                case BinaryOperator.Minus:
                    return "-";
                case BinaryOperator.Asterisk:
                    return "*";
                case BinaryOperator.Slash:
                    return "/";
                case BinaryOperator.EqualsEquals:
                    return "==";
                case BinaryOperator.NotEqual:
                    return "=/=";
                case BinaryOperator.LessThan:
                    return "<";
                case BinaryOperator.LessThanOrEqual:
                    return "<=";
                case BinaryOperator.GreaterThan:
                    return ">";
                case BinaryOperator.GreaterThanOrEqual:
                    return ">=";
                case BinaryOperator.And:
                    return "and";
                case BinaryOperator.Or:
                    return "or";
                case BinaryOperator.DotDot:
                    return "..";
                case BinaryOperator.LessThanDotDot:
                    return "<..";
                case BinaryOperator.DotDotLessThan:
                    return "..<";
                case BinaryOperator.LessThanDotDotLessThan:
                    return "<..<";
            }
        }
    }
}
