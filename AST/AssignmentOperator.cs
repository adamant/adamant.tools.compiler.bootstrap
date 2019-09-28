using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public enum AssignmentOperator
    {
        Simple,
        Plus,
        Minus,
        Asterisk,
        Slash
    }

    public static class AssignmentOperatorExtensions
    {
        public static string ToSymbolString(this AssignmentOperator @operator)
        {
            switch (@operator)
            {
                case AssignmentOperator.Simple:
                    return "=";
                case AssignmentOperator.Plus:
                    return "+=";
                case AssignmentOperator.Minus:
                    return "-=";
                case AssignmentOperator.Asterisk:
                    return "*=";
                case AssignmentOperator.Slash:
                    return "/=";
                default:
                    throw ExhaustiveMatch.Failed(@operator);
            }
        }
    }
}
