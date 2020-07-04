using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions
{
    public enum CompareInstructionOperator
    {
        Equal,
        NotEqual,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
    }

    public static class CompareInstructionOperatorExtensions
    {
        public static string ToInstructionString(this CompareInstructionOperator @operator)
        {
            return @operator switch
            {
                CompareInstructionOperator.Equal => "EQ",
                CompareInstructionOperator.NotEqual => "NE",
                CompareInstructionOperator.LessThan => "LT",
                CompareInstructionOperator.LessThanOrEqual => "LTE",
                CompareInstructionOperator.GreaterThan => "GT",
                CompareInstructionOperator.GreaterThanOrEqual => "GTE",
                _ => throw ExhaustiveMatch.Failed(@operator)
            };
        }
    }
}
