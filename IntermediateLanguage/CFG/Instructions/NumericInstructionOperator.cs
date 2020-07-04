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
            return @operator switch
            {
                NumericInstructionOperator.Add => "ADD",
                NumericInstructionOperator.Subtract => "SUB",
                NumericInstructionOperator.Multiply => "MUL",
                NumericInstructionOperator.Divide => "DIV",
                _ => throw ExhaustiveMatch.Failed(@operator)
            };
        }
    }
}
