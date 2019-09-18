using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public enum UnaryOperator
    {
        Question,
        Not,
        Minus,
        Plus,
        //At,
        //Caret,
    }

    public static class UnaryOperatorExtensions
    {
        public static string ToSymbolString(this UnaryOperator @operator)
        {
            switch (@operator)
            {
                //case UnaryOperator.At:
                //    return "@";
                case UnaryOperator.Not:
                    return "not ";
                //case UnaryOperator.Caret:
                //    return "^";
                case UnaryOperator.Minus:
                    return "-";
                case UnaryOperator.Plus:
                    return "+";
                case UnaryOperator.Question:
                    return "?";
                default:
                    throw ExhaustiveMatch.Failed(@operator);
            }
        }
    }
}
