using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public enum AssignmentOperator
    {
        Direct,
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
                case AssignmentOperator.Direct:
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
                    throw NonExhaustiveMatchException.For(@operator);
            }
        }
    }
}
