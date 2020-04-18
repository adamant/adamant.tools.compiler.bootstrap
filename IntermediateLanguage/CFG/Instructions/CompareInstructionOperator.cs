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
            switch (@operator)
            {
                default:
                    throw ExhaustiveMatch.Failed(@operator);
                case CompareInstructionOperator.Equal:
                    return "EQ";
                case CompareInstructionOperator.NotEqual:
                    return "NE";
                case CompareInstructionOperator.LessThan:
                    return "LT";
                case CompareInstructionOperator.LessThanOrEqual:
                    return "LTE";
                case CompareInstructionOperator.GreaterThan:
                    return "GT";
                case CompareInstructionOperator.GreaterThanOrEqual:
                    return "GTE";
            }
        }
    }
}
