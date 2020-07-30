using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public enum AccessOperator
    {
        Standard,
        Conditional,
    }

    public static class AccessOperatorExtensions
    {
        public static string ToSymbolString(this AccessOperator @operator)
        {
            return @operator switch
            {
                AccessOperator.Standard => ".",
                AccessOperator.Conditional => "?.",
                _ => throw ExhaustiveMatch.Failed(@operator)
            };
        }
    }
}
