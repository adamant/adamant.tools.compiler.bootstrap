using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public static class ExpressionSemanticsExtensions
    {
        public static string Action(this ExpressionSemantics valueSemantics)
        {
            string mutability = valueSemantics switch
            {
                ExpressionSemantics.Never => "never",
                ExpressionSemantics.Void => "void",
                ExpressionSemantics.Move => "move",
                ExpressionSemantics.Copy => "copy",
                ExpressionSemantics.Acquire => "own",
                ExpressionSemantics.Borrow => "borrow",
                ExpressionSemantics.Share => "share",
                _ => throw ExhaustiveMatch.Failed(valueSemantics),
            };

            return mutability;
        }
    }
}
