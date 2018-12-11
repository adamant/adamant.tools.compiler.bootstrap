using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
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
    }

    public static class BinaryOperatorExtensions
    {
        public static string ToSymbolString(this BinaryOperator @operator)
        {
            switch (@operator)
            {
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
                default:
                    throw NonExhaustiveMatchException.For(@operator);
            }
        }
    }
}
