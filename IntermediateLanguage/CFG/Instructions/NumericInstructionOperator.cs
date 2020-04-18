using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions
{
    public enum NumericInstructionOperator
    {
        Add,
        Subtract,
        Multiply,
        Divide,
    }

    public static class NumericInstructionOperatorExtensions
    {
        public static string ToInstructionString(this NumericInstructionOperator @operator)
        {
            switch (@operator)
            {
                default:
                    throw ExhaustiveMatch.Failed(@operator);
                case NumericInstructionOperator.Add:
                    return "ADD";
                case NumericInstructionOperator.Subtract:
                    return "SUB";
                case NumericInstructionOperator.Multiply:
                    return "MUL";
                case NumericInstructionOperator.Divide:
                    return "DIV";
            }
        }
    }
}
