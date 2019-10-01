using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
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
            switch (@operator)
            {
                default:
                    throw ExhaustiveMatch.Failed(@operator);
                case AccessOperator.Standard:
                    return ".";
                case AccessOperator.Conditional:
                    return "?.";
            }
        }
    }
}
