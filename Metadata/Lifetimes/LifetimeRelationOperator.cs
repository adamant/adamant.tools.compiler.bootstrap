using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public enum LifetimeRelationOperator
    {
        LessThanOrEqualTo,
        StrictlyLessThan,
        GreaterThanOrEqualTo,
        StrictlyGreaterThan
    }

    public static class LifetimeRelationOperatorExtensions
    {
        public static string ToSymbolString(this LifetimeRelationOperator @operator)
        {
            switch (@operator)
            {
                case LifetimeRelationOperator.LessThanOrEqualTo:
                    return "<";
                case LifetimeRelationOperator.StrictlyLessThan:
                    return "</=";
                case LifetimeRelationOperator.GreaterThanOrEqualTo:
                    return ">";
                case LifetimeRelationOperator.StrictlyGreaterThan:
                    return ">/=";
                default:
                    throw NonExhaustiveMatchException.For(@operator);
            }
        }
    }
}
