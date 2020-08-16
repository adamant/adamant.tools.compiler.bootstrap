using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Core.Operators
{
    public static class UnaryOperatorExtensions
    {
        public static string ToSymbolString(this UnaryOperator @operator)
        {
            switch (@operator)
            {
                default:
                    throw ExhaustiveMatch.Failed(@operator);
                case UnaryOperator.Not:
                    return "not ";
                case UnaryOperator.Minus:
                    return "-";
                case UnaryOperator.Plus:
                    return "+";
            }
        }
    }
}
