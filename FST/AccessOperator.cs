using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.FST
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
