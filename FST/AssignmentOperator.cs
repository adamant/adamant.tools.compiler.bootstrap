using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.CST
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
            return @operator switch
            {
                AssignmentOperator.Simple => "=",
                AssignmentOperator.Plus => "+=",
                AssignmentOperator.Minus => "-=",
                AssignmentOperator.Asterisk => "*=",
                AssignmentOperator.Slash => "/=",
                _ => throw ExhaustiveMatch.Failed(@operator)
            };
        }
    }
}
