using Adamant.Tools.Compiler.Bootstrap.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    [Closed(
        typeof(IImplicitNumericConversionExpression),
        typeof(IImplicitOptionalConversionExpression),
        typeof(IImplicitNoneConversionExpression),
        typeof(IImplicitImmutabilityConversionExpression))]
    public partial interface IImplicitConversionExpression : IExpressionSyntax
    {
        IExpressionSyntax Expression { get; }
        new DataType Type { get; }
    }
}
